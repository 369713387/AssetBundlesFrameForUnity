using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 对资源的Object进行封装
/// </summary>
public class TempObject{
    /// <summary>
    /// 资源列表
    /// </summary>
    private List<Object> assetList;
    /// <summary>
    /// 资源
    /// </summary>
    public List<Object> Asset
    {
        get
        {
            return assetList;
        }
    }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="assets"></param>
    public TempObject(params Object[] assets)
    {
        //新建一个资源列表，并初始化
        assetList = new List<Object>(assets);
    }
    /// <summary>
    /// 卸载资源
    /// </summary>
    public void UnLoadAsset()
    {
        //卸载资源的时候，要从列表的末尾开始删，否则会报错
        for (int i = assetList.Count - 1; i >= 0; i--)
            Resources.UnloadAsset(assetList[i]);
    }
}
