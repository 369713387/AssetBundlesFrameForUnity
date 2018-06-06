using UnityEngine;
/// <summary>
/// 资源管理
/// </summary>
public class AssetLoader : System.IDisposable{

    private AssetBundle m_assetBundle;//当前资源包

    public AssetBundle assetBundle    //（供外部调用）设置assetbundle的接口
    {
        set
        {
            m_assetBundle = value;
        }
    }
    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>Objcet对象</returns>
    public Object LoadAsset(string assetname)
    {
        if (m_assetBundle == null)
        {
            Debug.LogError("当前资源包为空，无法获取" + assetname + "资源");
            return null;
        }
        else if (!m_assetBundle.Contains(assetname))
        {
            Debug.LogError("当前资源包里不存在" + assetname + "资源");
            return null;
        }
        else
            return m_assetBundle.LoadAsset(assetname);
    }
    /// <summary>
    /// 获取assetbundle包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets()
    {
        if (m_assetBundle == null)
        {
            Debug.LogError("当前资源包为空，无法获取资源");
            return null;
        }
        else
            return m_assetBundle.LoadAllAssets();
    }
    /// <summary>
    /// 获取带有子物体的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string assetname)
    {
        if (m_assetBundle == null)
        {
            Debug.LogError("当前资源包为空，无法获取" + assetname + "资源");
            return null;
        }
        else if (!m_assetBundle.Contains(assetname))
        {
            Debug.LogError("当前资源包里不存在" + assetname + "资源");
            return null;
        }
        else
            return m_assetBundle.LoadAssetWithSubAssets(assetname);
    }
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(Object asset)
    {
        Resources.UnloadAsset(asset);
    }
    /// <summary>
    /// 释放内存中的资源包
    /// </summary>
    public void Dispose()
    {
        if (m_assetBundle == null)
            return;

        //false:只卸载 包
        //true:卸载 包 和 包里的Obj资源
        m_assetBundle.Unload(false);
    }
    /// <summary>
    /// 查看assetbundle里面所有资源的名称（调试专用）
    /// </summary>
    public void GetAllAssetNames()
    {
        string[] names = m_assetBundle.GetAllAssetNames();
        int index = 1;
        foreach (var item in names)
        {
            Debug.Log("asset" + index + ": " + item);
            index++;
        }
    }
}
