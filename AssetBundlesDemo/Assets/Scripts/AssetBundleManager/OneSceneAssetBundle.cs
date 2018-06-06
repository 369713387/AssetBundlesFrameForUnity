using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理一个场景里面所有的资源包
/// </summary>
public class OneSceneAssetBundle
{
    /// <summary>
    /// AB包名和对应AB包的依赖关系的字典映射
    /// </summary>
    private Dictionary<string, AssetBundleRelation> NameAndBundleDict;
    /// <summary>
    /// AB包名和对应包的缓存的字典映射
    /// </summary>
    private Dictionary<string, AssetCache> NameAndCacheDict;

    private string m_sceneName;//当前场景名

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="scenename"></param>
    public OneSceneAssetBundle(string scenename)
    {
        m_sceneName = scenename;

        NameAndBundleDict = new Dictionary<string, AssetBundleRelation>();
        NameAndCacheDict = new Dictionary<string, AssetCache>();
    }
    /// <summary>
    /// 添加加载AB包协程的对外接口
    /// </summary>
    public void LoadAssetBundle(string abname, LoadProgress lp, LoadAssetBundleCallBack labcb)
    {
        //如果这个包已经被加载了 输出一条提示信息
        if (NameAndBundleDict.ContainsKey(abname))
        {
            Debug.LogWarning("此包已经被加载了 ： " + abname);
        }
        else
        {
            //没有被加载过  则进行加载
            AssetBundleRelation abrelation = new AssetBundleRelation(abname, lp);
            //将AB包和其对应的依赖关系保存到字典中
            NameAndBundleDict.Add(abname, abrelation);
            //开始进行加载
            //如果这个类继承了Mono的话，我们是通过开启协程来加载的。如 ： StartCoroutine("Load",abname);
            //现在我们不想继承Mono,又想加载AB包的话，我们可以通过一个回调函数，把参数传递给上层，让上层来进行处理
            labcb(m_sceneName, abname);
        }
    }
    /// <summary>
    /// 是否加载了指定的AB包
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    public bool isLoaded(string abname)
    {
        return NameAndBundleDict.ContainsKey(abname);
    }

    /// <summary>
    /// 是否加载完成指定的AB包
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    public bool isLoadCompleted(string abname)
    {
        if (isLoaded(abname))
        {
            return NameAndBundleDict[abname].FinishLoad;
        }
        else
            return false;
    }

    #region 加载AB包
    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <returns></returns>
    public IEnumerator Load(string assetbundlename)
    {
        //AssetBundleManifest文件加载完成后才能进行AssetBundle的加载操作
        while (!AssetBundleManifestLoader.instance.Finish)
        {
            yield return null;
        }

        AssetBundleRelation assetBundleRelation = NameAndBundleDict[assetbundlename];
        //先获取这个包的所有依赖关系
        string[] dependencieBundles = AssetBundleManifestLoader.instance.GetDependencies(assetbundlename);
        //添加它的依赖关系
        foreach (var dependencieBundleName in dependencieBundles)
        {
            assetBundleRelation.AddDependencie(dependencieBundleName);
            //加载这个包的所有依赖关系（注意这里的参数）
            yield return LoadDependencie(dependencieBundleName, assetbundlename, assetBundleRelation.lp);
        }
        //开始加载某个AB包
        yield return assetBundleRelation.Load();
    }
    /// <summary>
    /// 加载某个AB包依赖的包
    /// </summary>
    /// <param name="assetbundlename">需要加载的AB包所依赖的包</param>
    /// <param name="referencebundlename">AB包自身</param>
    /// <param name="lp"></param>
    /// <returns></returns>
    private IEnumerator LoadDependencie(string assetbundlename, string referencebundlename, LoadProgress lp)
    {
        //判断是否已经加载过它依赖的包
        if (NameAndBundleDict.ContainsKey(assetbundlename))
        {
            //已经加载过 直接添加他的被依赖关系
            AssetBundleRelation abrelation = NameAndBundleDict[assetbundlename];
            
            //添加这个包的被依赖关系
            abrelation.AddReference(referencebundlename);
        }
        else
        {
            //没有加载过 就创建一个新的依赖关系
            AssetBundleRelation abrelation = new AssetBundleRelation(assetbundlename, lp);
            //添加这个包的被依赖关系
            abrelation.AddReference(referencebundlename);
            //将AB包名和其对应的AB包的依赖关系保存到字典中
            NameAndBundleDict.Add(assetbundlename, abrelation);
            //加载依赖的包
            yield return Load(assetbundlename);
        }
    }
    #endregion

    #region 1.加载资源 2.卸载资源 3.卸载AB包

    //如果我们是第一次获取资源，我们就通过调用最底层的AssetBundle.LoadAsset(assetName),来获取资源
    //如果我们是第二次获取同一个资源，再次调用底层方法，则会浪费系统资源
    //我们可以通过把第一次获取到的资源缓存起来，供下一次使用，这样可以节约系统资源
    //所以我们需要添加一个缓存层，优化获取资源的方法

    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>Objcet对象</returns>
    public Object LoadAsset(string assetbundlename, string assetname)
    {
        //先判断缓存层有没有缓存
        if (NameAndCacheDict.ContainsKey(assetbundlename))
        {
            //有缓存直接获取资源
            Object[] assets = NameAndCacheDict[assetbundlename].GetAsset(assetname);
            //安全校验，排除缓存层存在但是资源不存在的情况
            if (assets != null)
                return assets[0];//因为是获取单个资源，所以返回第一个Object
        }
        //当前AB包没有被加载
        if (!NameAndBundleDict.ContainsKey(assetbundlename))
        {
            Debug.LogError("当前" + assetbundlename + "包没有加载，无法获取资源");
            return null;
        }
        else
        {
            //当前AB包已经被加载了,则获取资源
            Object asset = NameAndBundleDict[assetbundlename].LoadAsset(assetname);
            //封装该资源
            TempObject tempAsset = new TempObject(asset);
            //缓存层已经被创建，缓存层里也有资源，但是我们要获取的那个资源不存在缓存层中
            if (NameAndCacheDict.ContainsKey(assetbundlename))
            {
                //直接把资源加入缓存层中
                NameAndCacheDict[assetbundlename].AddAsset(assetname, tempAsset);
            }
            else
            {
                //第一次获取AB包里的资源，缓存层没有被创建，则新建一个缓存层
                AssetCache cache = new AssetCache();
                //把资源加入缓存层中
                cache.AddAsset(assetname, tempAsset);
                //将缓存层保存到字典里
                NameAndCacheDict.Add(assetbundlename, cache);
            }
            //返回获取到的资源
            return asset;
        }
    }
    /// <summary>
    /// 获取assetbundle包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets(string assetbundlename)
    {
        //TODO
        //没有实现添加进缓存层的判断
        if (!NameAndBundleDict.ContainsKey(assetbundlename))
        {
            Debug.LogError("当前" + assetbundlename + "包没有加载，无法获取资源");
            return null;
        }
        else
        {
            return NameAndBundleDict[assetbundlename].LoadAllAssets();
        }
    }
    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string assetbundlename, string assetname)
    {
        //先判断缓存层有没有缓存
        if (NameAndCacheDict.ContainsKey(assetbundlename))
        {
            //有缓存直接获取资源
            Object[] assets = NameAndCacheDict[assetbundlename].GetAsset(assetname);
            //安全校验，排除缓存层存在但是资源不存在的情况
            if (assets != null)
                return assets;//因为是获取带有子物体的资源，所以返回Object数组
        }
        //当前AB包没有被加载
        if (!NameAndBundleDict.ContainsKey(assetbundlename))
        {
            Debug.LogError("当前" + assetbundlename + "包没有加载，无法获取资源");
            return null;
        }
        else
        {
            //当前AB包已经被加载了,则获取带有子物体的资源数组
            Object[] asset = NameAndBundleDict[assetbundlename].LoadAssetWithSubAssets(assetname);
            //封装该资源
            TempObject tempAsset = new TempObject(asset);
            //缓存层已经被创建，缓存层里也有资源，但是我们要获取的那个资源不存在缓存层中
            if (NameAndCacheDict.ContainsKey(assetbundlename))
            {
                //直接把资源加入缓存层中
                NameAndCacheDict[assetbundlename].AddAsset(assetname, tempAsset);
            }
            else
            {
                //第一次获取AB包里的资源，缓存层没有被创建，则新建一个缓存层
                AssetCache cache = new AssetCache();
                //把资源加入缓存层中
                cache.AddAsset(assetname, tempAsset);
                //将缓存层保存到字典里
                NameAndCacheDict.Add(assetbundlename, cache);
            }
            //返回带有子物体的资源数组
            return asset;
        }
    }



    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(string assetbundlename, string assetname)
    {
        if (!NameAndCacheDict.ContainsKey(assetbundlename))
        {
            Debug.LogError("当前" + assetbundlename + "包没有缓存资源，无法卸载资源");
        }
        else
            NameAndCacheDict[assetbundlename].UnLoadAsset(assetname);
        //卸载没有用到的资源
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 卸载一个包里面的所有资源
    /// </summary>
    /// <param name="abname"></param>
    public void UnLoadAllAsset(string abname)
    {
        if (!NameAndCacheDict.ContainsKey(abname))
        {
            Debug.LogError("当前" + abname + "包没有缓存资源，无法卸载资源");
        }
        else
        {
            NameAndCacheDict[abname].UnLoadAllAsset();
            //移除映射关系
            NameAndCacheDict.Remove(abname);
            //卸载没有用到的资源
            Resources.UnloadUnusedAssets();
        }            
    }

    /// <summary>
    /// 卸载所有包里的所有资源
    /// </summary>
    public void UnLoadAll()
    {
        foreach (string key in NameAndCacheDict.Keys)
        {
            UnLoadAllAsset(key);
        }

        NameAndCacheDict.Clear();
    }

    /// <summary>
    /// 卸载内存中的单个AB包
    /// </summary>
    public void Dispose(string assetbundlename)
    {
        if (!NameAndBundleDict.ContainsKey(assetbundlename))
        {
            Debug.LogError("当前" + assetbundlename + "包没有加载，无法获取资源");
            return;
        }

        //先获取到当前需要卸载的包
        AssetBundleRelation abrelation = NameAndBundleDict[assetbundlename];
        //再获取当前包的所有依赖关系
        string[] allDependencies = abrelation.GetAllDependencies();
        //遍历每个依赖关系,处理当前包的依赖和被依赖关系
        foreach (string Dabname in allDependencies)
        {
            //获取当前包的依赖包的被依赖关系
            AssetBundleRelation referenceAB = NameAndBundleDict[Dabname];
            //如果当前包的依赖包,存在被依赖关系
            if (referenceAB.RemoveReference(assetbundlename))
            {
                //我们要先卸载被依赖包，并移除被依赖关系
                //递归
                Dispose(referenceAB.abName);
            }
        }
        //再次判断，确保当前包没有被依赖关系
        if (abrelation.GetAllReferences().Length <= 0)
        {
            //开始卸载当前包
            NameAndBundleDict[assetbundlename].Dispose();
            //移除当前包的字典映射
            NameAndBundleDict.Remove(assetbundlename);
        }

    }
    /// <summary>
    /// 卸载所有AB包
    /// </summary>
    public void DisposeAll()
    {
        foreach (string key in NameAndBundleDict.Keys)
        {
            Dispose(key);
        }

        NameAndBundleDict.Clear();
    }

    /// <summary>
    /// 卸载所有的AB包和所有的资源
    /// </summary>
    public void DisposeAllAndUnLoadAllAsset()
    {
        UnLoadAll();
        DisposeAll();       
    }
    #endregion


    /// <summary>
    /// 查看assetbundle里面所有资源的名称（调试专用）
    /// </summary>
    public void GetAllAssetNames(string assetbundlename)
    {
        NameAndBundleDict[assetbundlename].GetAllAssetNames();
    }
}
