using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 加载进度
/// </summary>
/// <param name="assetbundlename"></param>
/// <param name="progress"></param>
public delegate void LoadProgress(string assetbundlename, float progress);
/// <summary>
/// 加载完成
/// </summary>
/// <param name="assetbundlename"></param>
public delegate void LoadCompleted(string assetbundlename);

/// <summary>
/// 加载AB包的回调函数
/// </summary>
/// <param name="scenename">场景名</param>
/// <param name="assetbundlename">AB包名</param>
public delegate void LoadAssetBundleCallBack(string scenename,string assetbundlename);
public class AssetUtil { 

}
