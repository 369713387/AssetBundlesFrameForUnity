using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 资源缓存层
/// </summary>
public class AssetCache{
    /// <summary>
    /// 已经加载过的资源名字和资源的字典映射关系
    /// </summary>
    private Dictionary<string,TempObject> NameAndAssetDict;
    /// <summary>
    /// 构造函数
    /// </summary>
    public AssetCache()
    {
        NameAndAssetDict = new Dictionary<string,TempObject>();
    }
    /// <summary>
    /// 添加缓存
    /// </summary>
    /// <param name="assetname">资源名字</param>
    /// <param name="asset">资源</param>
    public void AddAsset(string assetname,TempObject asset)
    {
        if (NameAndAssetDict.ContainsKey(assetname))
        {
            Debug.LogError("此"+assetname+"资源已经被加载！");
            return;
        }
        else
        {
            //缓存资源
            NameAndAssetDict.Add(assetname, asset);
        }
    }
    /// <summary>
    /// 获取缓存的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns></returns>
    public Object[] GetAsset(string assetname)
    {
        if (NameAndAssetDict.ContainsKey(assetname))
        {
            //获取资源
            return NameAndAssetDict[assetname].Asset.ToArray();
        }
        else
        {
            Debug.LogError("此" + assetname + "资源已经被加载！");
            return null;
        }
    }
    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="assetname"></param>
    public void UnLoadAsset(string assetname){
        if (NameAndAssetDict.ContainsKey(assetname))
        {
            NameAndAssetDict[assetname].UnLoadAsset();
        }
        else
        {
            Debug.LogError("此" + assetname + "资源已经被加载！");
            return;
        }
    }
    /// <summary>
    /// 卸载所有资源
    /// </summary>
    public void UnLoadAllAsset()
    {
        //卸载所有资源
        foreach(string assetname in NameAndAssetDict.Keys)
        {
            UnLoadAsset(assetname);
        }
        //清空他们的映射关系
        NameAndAssetDict.Clear();
    }
}
