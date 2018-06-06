using System.Collections;
using UnityEngine;
/// <summary>
/// AB包的管理
/// </summary>
public class AssetBundleLoader{

    private AssetLoader m_assetLoader;//AB包里的资源，即下层对象
    private WWW m_www;//WWW对象
    private string m_assetBundleName;//AB包名
    private string m_assetBundlePath;//AB包的路径
    private float m_progress;//WWW加载进度
    private LoadProgress m_lp;//加载进度回调函数
    private LoadCompleted m_lc;//加载完成回调函数

    /// <summary>
    /// ABloader构造函数
    /// </summary>
    /// <param name="assetbundlename">AB包名</param>
    /// <param name="lc">加载完成回调</param>
    /// <param name="lp">加载进度回调</param>
    public AssetBundleLoader(string assetbundlename,LoadCompleted lc ,LoadProgress lp)
    {
        m_assetLoader = null;
        m_www = null;
        m_assetBundleName = assetbundlename;
        m_assetBundlePath = PathUtil.getWWWPath() + "/" + m_assetBundleName;
        m_progress = 0.0f;
        m_lp = lp;
        m_lc = lc;
    }
    /// <summary>
    /// 加载资源包
    /// </summary>
    /// <returns></returns>
    public IEnumerator LoadAssetBundle()
    {
        //构造一个www对象
        m_www = new WWW(m_assetBundlePath);
        //等待www加载完成
        while (!m_www.isDone)
        {
            m_progress = m_www.progress;
            //每一帧调用一次，更新加载进度
            if (m_lp != null)
            {
                m_lp(m_assetBundleName, m_progress);
            }
            yield return m_www;
        }
        //再次判断，保证www加载完成
        m_progress = m_www.progress;
        if(m_progress>=1f)
        {
            //加载完成后的操作
            //给下层对象赋值
            m_assetLoader = new AssetLoader();
            m_assetLoader.assetBundle = m_www.assetBundle;
            //每一帧调用一次，更新加载进度
            if (m_lp != null)
            {
                m_lp(m_assetBundleName, m_progress);
            }
            //加载完成
            if(m_lc != null)
            {
                m_lc(m_assetBundleName);
            }
        }
    }

    #region 重新调用下层的函数
    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>Objcet对象</returns>
    public Object LoadAsset(string assetname)
    {
        if (m_assetLoader == null)
        {
            Debug.LogError("当前m_assetLoader为空，无法获取" + assetname + "资源");
            return null;
        }
        else
            return m_assetLoader.LoadAsset(assetname);
    }
    /// <summary>
    /// 获取assetbundle包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets()
    {
        if (m_assetLoader == null)
        {
            Debug.LogError("当前m_assetLoader为空，无法获取资源");
            return null;
        }
        else
            return m_assetLoader.LoadAllAssets();
    }
    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string assetname)
    {
        if (m_assetLoader == null)
        {
            Debug.LogError("当前m_assetLoader空，无法获取" + assetname + "资源");
            return null;
        }
        else
            return m_assetLoader.LoadAssetWithSubAssets(assetname);
    }
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(Object asset)
    {
        if (m_assetLoader == null)
        {
            Debug.LogError("当前m_assetLoader空");
            return;
        }
        else
            m_assetLoader.UnLoadAsset(asset);
    }
    /// <summary>
    /// 释放内存中的资源包
    /// </summary>
    public void Dispose()
    {
        if (m_assetLoader == null)
            return;

        m_assetLoader.Dispose();
        m_assetLoader = null;
    }
    /// <summary>
    /// 查看assetbundle里面所有资源的名称（调试专用）
    /// </summary>
    public void GetAllAssetNames()
    {
        m_assetLoader.GetAllAssetNames();
    }
    #endregion
}
