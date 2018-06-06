using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathUtil {
    /// <summary>
    /// 获取assetbundle的输出路径
    /// </summary>
    /// <returns></returns>
	public static string GetAssetBundleOutPath()
    {
        string outpath = getPlatformPath() + "/" + getPlatformName();

        if (!Directory.Exists(outpath))
            Directory.CreateDirectory(outpath);

        return outpath;
    }
    /// <summary>
    /// 自动获取平台的输出路径
    /// </summary>
    /// <returns></returns>
    private static string getPlatformPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return Application.streamingAssetsPath; 
            case RuntimePlatform.Android:
                return Application.persistentDataPath;
            default:
                return null;
        }
    }
    /// <summary>
    /// 自动获取对应平台的名称
    /// </summary>
    /// <returns></returns>
    public static string getPlatformName()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            case RuntimePlatform.Android:
                return "Android";
            default:
                return null;
        }
    }

    public static string getWWWPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "file:///" + GetAssetBundleOutPath() ;
            case RuntimePlatform.Android:
                return "jar:file://" + GetAssetBundleOutPath();
            default:
                return null;
        }
    }
}
