using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// AssetBundleManifestLoader
/// </summary>
public class AssetBundleManifestLoader {

    /// <summary>
    /// 单例模式，考虑了线程安全
    /// </summary>
    private static AssetBundleManifestLoader m_instance;
    private static object _lock = new object();
    public static AssetBundleManifestLoader instance
    {
        get
        {
            if(m_instance == null)
            {
                lock (_lock)
                {
                    if(m_instance == null)
                    {
                        m_instance = new AssetBundleManifestLoader();
                    }
                }
            }
            return m_instance;
        }
    }

    private AssetBundleManifest m_manifest;//manifest文件
    private string m_manifestPath;//manifest路径
    private AssetBundle m_assetbundle;//全局存在的AB包
    private bool m_Finish;//是否加载完成标志位
    public bool Finish    //是否加载完成标志位,封装
    {
        get
        {
            return m_Finish;
        }
    }
    /// <summary>
    /// ABManifestLoader构造函数
    /// </summary>
    private AssetBundleManifestLoader()
    {
        m_manifestPath = PathUtil.getWWWPath() + "/" + PathUtil.getPlatformName();
        m_manifest = null;
        m_assetbundle = null;
        m_Finish = false;
    }
    /// <summary>
    /// 加载AssetBundleManifest
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadAssetBundleManifest()
    {
        //通过WWW对象来加载
        WWW www = new WWW(m_manifestPath);
        yield return www;
        //错误判断
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError("加载Manifest文件出错 ： " + www.error);
        }
        else
        {
            if (www.progress >= 1.0f)
            {
                //加载完成，给类中的属性赋值
                m_assetbundle = www.assetBundle;
                m_manifest = m_assetbundle.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
                m_Finish = true;
            }
        }
    }
    /// <summary>
    /// 获取某个AB包中的所有的依赖关系
    /// </summary>
    /// <param name="assetbundlename">包名</param>
    /// <returns></returns>
    public string[] GetDependencies(string assetbundlename)
    {
        return m_manifest.GetAllDependencies(assetbundlename);
    }
    /// <summary>
    /// 卸载AssetBundleManifest
    /// </summary>
    public void UnLoadAssetBundleManifest()
    {
        m_assetbundle.Unload(true);
    }
}
