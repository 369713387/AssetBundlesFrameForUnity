using System.Collections.Generic;
using UnityEngine;

public class AssetBundleManager : MonoBehaviour {

    /// <summary>
    /// 单例模式，考虑了线程安全
    /// </summary>
    private static AssetBundleManager m_instance;
    private static object _lock = new object();
    public static AssetBundleManager instance
    {
        get
        {
            if (m_instance == null)
            {
                lock (_lock)
                {
                    if (m_instance == null)
                    {
                        m_instance = new AssetBundleManager();
                    }
                }
            }
            return m_instance;
        }
    }

    private void Awake()
    {
        m_instance = this;
        //先加载manifest文件
        StartCoroutine(AssetBundleManifestLoader.instance.LoadAssetBundleManifest());
    }

    private void OnDestroy()
    {
        NameAndSceneDict.Clear();
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    /// <summary>
    /// 场景名和场景里面所有包的管理的映射
    /// </summary>
    Dictionary<string, SceneManager> NameAndSceneDict = new Dictionary<string, SceneManager>();

    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="scenename">场景名</param>
    /// <param name="foldername">文件夹名</param>
    /// <param name="lp">加载进度回调</param>
    public void LoadAssetBundle(string scenename,string foldername,LoadProgress lp)
    {
        if (!NameAndSceneDict.ContainsKey(scenename))
        {
            //如果没有这个场景，先创建这个场景，再读取
            CreateSceneAssetBundles(scenename);
        }
        SceneManager sm = NameAndSceneDict[scenename];
        //开始加载场景里的AB包
        sm.LoadAssetBundle(foldername, lp, loadAssetBundleCallBack);
    }

    /// <summary>
    /// 根据文件夹名，获取完整的AB包名
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public string GetBundleName(string scenename,string foldername)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            return sm.GetBundleName(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
            return null;
        }
    }
    #region 集成底层方法

    /// <summary>
    /// 是否加载了指定的AB包
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public bool isLoaded(string scenename, string foldername)
    {
        if (NameAndSceneDict.ContainsKey(foldername))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            return sm.isLoaded(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
            return false;
        }
    }

    /// <summary>
    /// 是否加载完成指定的AB包
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public bool isLoadCompleted(string scenename, string foldername)
    {
        if (NameAndSceneDict.ContainsKey(foldername))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            return sm.isLoadCompleted(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
            return false;
        }
    }

    /// <summary>
    /// 根据文件夹名获取单个资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>Objcet对象</returns>
    public Object LoadAsset(string scenename,string foldername, string assetname)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            return sm.LoadAsset(foldername,assetname);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
            return null;
        }
    }

    /// <summary>
    /// 根据文件夹名获取assetbundle包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets(string scenename, string foldername)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            return sm.LoadAllAssets(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
            return null;
        }
    }

    /// <summary>
    /// 根据文件夹名获取带有子物体的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string scenename, string foldername, string assetname)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            return sm.LoadAssetWithSubAssets(foldername, assetname);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
            return null;
        }
    }



    /// <summary>
    /// 根据文件夹名卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(string scenename, string foldername, string assetname)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.UnLoadAsset(foldername, assetname);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }

    /// <summary>
    /// 卸载一个包里面的所有资源
    /// </summary>
    /// <param name="foldername"></param>
    public void UnLoadAllAsset(string scenename, string foldername)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.UnLoadAllAsset(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }

    /// <summary>
    /// 卸载所有包里的所有资源
    /// </summary>
    public void UnLoadAll(string scenename)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.UnLoadAll();
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }





    /// <summary>
    /// 卸载内存中的单个AB包
    /// </summary>
    public void Dispose(string scenename, string foldername)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.Dispose(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }

    /// <summary>
    /// 卸载所有AB包
    /// </summary>
    public void DisposeAll(string scenename)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.DisposeAll();
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }

    /// <summary>
    /// 卸载所有的AB包和所有的资源
    /// </summary>
    public void DisposeAllAndUnLoadAllAsset(string scenename)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.DisposeAllAndUnLoadAllAsset();
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }


    /// <summary>
    /// 查看assetbundle里面所有资源的名称（调试专用）
    /// </summary>
    public void GetAllAssetNames(string scenename, string foldername)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            sm.GetAllAssetNames(foldername);
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }
    #endregion    

    /// <summary>
    /// 创建一个场景对应的AB包
    /// </summary>
    /// <param name="scenename"></param>
    private void CreateSceneAssetBundles(string scenename)
    {
        SceneManager sm = new SceneManager(scenename);
        //读取记录文件
        sm.ReadRecord(scenename);
        //添加映射关系
        NameAndSceneDict.Add(scenename, sm);
    }

    /// <summary>
    /// 加载AB包的回调函数
    /// </summary>
    /// <param name="scenename">场景名</param>
    /// <param name="assetbundlename">AB包名</param>
    private void loadAssetBundleCallBack(string scenename, string assetbundlename)
    {
        if (NameAndSceneDict.ContainsKey(scenename))
        {
            SceneManager sm = NameAndSceneDict[scenename];
            //加载AB包
            StartCoroutine(sm.Load(assetbundlename));
        }
        else
        {
            Debug.LogError("不存在这个场景 ： " + scenename);
        }
    }
}
