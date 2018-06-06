using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AB包的依赖关系
/// </summary>
public class AssetBundleRelation
{

    private AssetBundleLoader m_ABloader;//AB包，即下层对象
    private string m_assetBundleName;//AB包名
    public string abName             //AB包名,封装
    {
        get
        {
            return m_assetBundleName;
        }
    }
    private LoadProgress m_lp;//加载进度回调函数
    public LoadProgress lp    //加载进度回调函数,封装
    {
        get
        {
            return m_lp;
        }
    }
    private bool m_FinishLoad;//是否加载完成标志位
    private List<string> DependencieBundle;//所有依赖的包名
    private List<string> ReferenceBundle;//所有被依赖的包名

    public bool FinishLoad    //是否加载完成标志位,封装
    {
        get
        {
            return m_FinishLoad;
        }
    }
    /// <summary>
    /// AB包的依赖关系
    /// </summary>
    /// <param name="assetbundlename"></param>
    /// <param name="lp"></param>
    public AssetBundleRelation(string assetbundlename, LoadProgress lp)
    {
        m_ABloader = new AssetBundleLoader(assetbundlename, OnLoadCompleted, lp);
        m_assetBundleName = assetbundlename;
        m_lp = lp;
        m_FinishLoad = false;
        DependencieBundle = new List<string>();
        ReferenceBundle = new List<string>();
    }
    /// <summary>
    /// 加载完成的回调
    /// </summary>
    private void OnLoadCompleted(string assetbundlename)
    {
        m_FinishLoad = true;
    }
    /// <summary>
    /// 加载资源包
    /// </summary>
    /// <returns></returns>
    public IEnumerator Load()
    {
        yield return m_ABloader.LoadAssetBundle();
    }

    #region 依赖关系
    /// <summary>
    /// 添加依赖关系
    /// </summary>
    /// <param name="assetbundlename">AB包名</param>
    public void AddDependencie(string assetbundlename)
    {
        //如果AB包名为空，或者AB包名已经添加了依赖关系，直接返回
        if (string.IsNullOrEmpty(assetbundlename))
            return;
        else if (DependencieBundle.Contains(assetbundlename))
            return;
        else
            DependencieBundle.Add(assetbundlename);//添加依赖关系
    }
    /// <summary>
    /// 移除依赖关系
    /// </summary>
    /// <param name="assetbundlename">AB包名</param>
    public void RemoveDependencie(string assetbundlename)
    {
        if (DependencieBundle.Contains(assetbundlename))
            DependencieBundle.Remove(assetbundlename);
    }
    /// <summary>
    /// 获取所有依赖关系
    /// </summary>
    /// <returns></returns>
    public string[] GetAllDependencies()
    {
        return DependencieBundle.ToArray();
    }
    #endregion

    #region 被依赖关系
    /// <summary>
    /// 添加被依赖关系
    /// </summary>
    /// <param name="assetbundlename">AB包名</param>
    public void AddReference(string assetbundlename)
    {
        //如果AB包名为空，或者AB包名已经添加了依赖关系，直接返回
        if (string.IsNullOrEmpty(assetbundlename))
            return;
        else if (ReferenceBundle.Contains(assetbundlename))
            return;
        else
            ReferenceBundle.Add(assetbundlename);//添加依赖关系
    }

    /// <summary>
    /// 移除被依赖关系
    /// </summary>
    /// <param name="assetbundlename">AB包名</param>
    /// <returns>true </returns>
    public bool RemoveReference(string assetbundlename)
    {
        if (ReferenceBundle.Contains(assetbundlename))
        {
            ReferenceBundle.Remove(assetbundlename);
            //移除一个包的时候，我们需要判断它是否还有被依赖的关系           
            if (ReferenceBundle.Count > 0)
            {
                //如果有，则无需做任何处理
                return true;
            }
            else
            {
                //如果没有，我们返回一个true,在上层中，需要从内存中释放这个包
                return false;
            }
        }
        else
        {
            return false;
        }

    }
    /// <summary>
    /// 获取所有被依赖关系
    /// </summary>
    /// <returns></returns>
    public string[] GetAllReferences()
    {
        return ReferenceBundle.ToArray();
    }
    #endregion

    #region 重新调用下层的函数
    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>Objcet对象</returns>
    public Object LoadAsset(string assetname)
    {
        if (m_ABloader == null)
        {
            Debug.LogError("当前m_ABloader为空，无法获取" + assetname + "资源");
            return null;
        }
        else
            return m_ABloader.LoadAsset(assetname);
    }
    /// <summary>
    /// 获取assetbundle包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets()
    {
        if (m_ABloader == null)
        {
            Debug.LogError("当前m_assetLoader为空，无法获取资源");
            return null;
        }
        else
            return m_ABloader.LoadAllAssets();
    }
    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string assetname)
    {
        if (m_ABloader == null)
        {
            Debug.LogError("当前m_ABloader空，无法获取" + assetname + "资源");
            return null;
        }
        else
            return m_ABloader.LoadAssetWithSubAssets(assetname);
    }
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(Object asset)
    {
        if (m_ABloader == null)
        {
            Debug.LogError("当前m_ABloader空");
            return;
        }
        else
            m_ABloader.UnLoadAsset(asset);
    }
    /// <summary>
    /// 释放内存中的资源包
    /// </summary>
    public void Dispose()
    {
        if (m_ABloader == null)
            return;

        m_ABloader.Dispose();
        m_ABloader = null;
    }
    /// <summary>
    /// 查看assetbundle里面所有资源的名称（调试专用）
    /// </summary>
    public void GetAllAssetNames()
    {
        m_ABloader.GetAllAssetNames();
    }
    #endregion
}
