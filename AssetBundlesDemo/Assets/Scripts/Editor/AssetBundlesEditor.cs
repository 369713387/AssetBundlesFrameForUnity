using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
/// <summary>
/// AssetBundles编辑器
/// </summary>
public class AssetBundlesEditor
{

    //思路：
    //1.找到资源保存的文件夹
    //2.遍历里面的每个场景文件夹
    //3.遍历场景文件夹里的所有文件系统
    //4.如果访问的是文件夹，继续访问里面的所有文件系统，直到找到文件（递归思想）
    //5.找到文件  修改它的AssetBundles labels
    //6.用AssetImproter类 修改名称和后缀
    //7.保存对应的文件夹名和具体路径
    #region 自动做标记
    [MenuItem("AssetBundle/Set AssetBundles Labels")]
    public static void SetAssetBundlesLabels()
    {
        //移除所有没有用的标签
        AssetDatabase.RemoveUnusedAssetBundleNames();
        //1.找到资源保存的文件夹
        string assetDirectory = Application.dataPath + "/Resource";

        DirectoryInfo directoryInfo = new DirectoryInfo(assetDirectory);
        DirectoryInfo[] sceneDirectories = directoryInfo.GetDirectories();
        Dictionary<string, string> namePathDict = new Dictionary<string, string>();
        //2.遍历里面的每个场景文件夹
        foreach (DirectoryInfo info in sceneDirectories)
        {
            string sceneDirectory = assetDirectory + "/" + info.Name;
            DirectoryInfo sceneDirectoryInfo = new DirectoryInfo(sceneDirectory);
            //错误检测
            if (sceneDirectoryInfo == null)
            {
                Debug.LogError(sceneDirectory + "不存在");
                return;
            }
            else
            {
                //3.遍历场景文件夹里的所有文件系统
                int index = sceneDirectory.LastIndexOf("/");
                string SceneName = sceneDirectory.Substring(index+1);
                //Debug.Log("场景目录 : "+sceneDirectory);
                //Debug.Log("场景名 : "+SceneName);
                OnSceneFileSystemInfo(sceneDirectoryInfo, SceneName,namePathDict);

                //7.保存对应的文件夹名和具体路径
                OnWriteConfig(SceneName, namePathDict);
            }
        }
        //刷新界面
        AssetDatabase.Refresh();

        Debug.Log("设置生成");
    }

    /// <summary>
    /// 记录配置文件信息
    /// </summary>
    /// <param name="sceneDirectory"></param>
    /// <param name="namePathDict"></param>
    private static void OnWriteConfig(string scenename , Dictionary<string,string> namePathDict)
    {
        //输出文件路径
        string path = PathUtil.GetAssetBundleOutPath() + "/" + scenename + "Record.config";
        //用字节流写入来存储信息
        using (FileStream fs = new FileStream(path,FileMode.OpenOrCreate,FileAccess.Write))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                //1.记录文件夹的个数，即字典中关系的数量
                sw.WriteLine(namePathDict.Count);
                //2.记录文件夹名和具体路径的字典key-value关系
                foreach (KeyValuePair<string, string> kv in namePathDict)
                    sw.WriteLine(kv.Key + "-" + kv.Value);
            }
        }
    }


    /// <summary>
    /// 遍历场景文件夹里所有的文件
    /// </summary>
    /// <param name="fsinfo"></param>
    /// <param name="scenename"></param>
    private static void OnSceneFileSystemInfo(FileSystemInfo fsinfo,string scenename,Dictionary<string,string> namepathdict)
    {
        //文件信息检查
        if (!fsinfo.Exists)
        {
            Debug.LogError(fsinfo.FullName + "不存在");
            return;
        }
        //文件夹信息
        DirectoryInfo dirInfo = fsinfo as DirectoryInfo;
        //文件夹下的所有的文件信息
        FileSystemInfo[] fsInfos = dirInfo.GetFileSystemInfos();
        //遍历文件夹下的所有的文件信息
        foreach (var tempfsInfo in fsInfos)
        {
            FileInfo fileInfo = tempfsInfo as FileInfo;
            if(fileInfo == null)
            {
                //转换为文件类型失败，代表这是一个文件夹
                //4.如果访问的是文件夹，继续访问里面的所有文件系统，直到找到文件（递归思想）
                OnSceneFileSystemInfo(tempfsInfo, scenename,namepathdict);
            }
            else
            {
                //遍历的这个对象是文件
                //5.找到文件  修改它的AssetBundles labels
                setLabels(fileInfo, scenename, namepathdict);
            }
        }
    }

    /// <summary>
    /// 设置AssetBundle的Labels
    /// </summary>
    /// <param name="fileinfo"></param>
    /// <param name="scenename"></param>
    private static void setLabels(FileInfo fileinfo,string scenename,Dictionary<string,string> namepathdict)
    {
        //对Unity自身生成的meta文件进行忽视
        if (fileinfo.Extension == ".meta")
            return;
        //获取AB包名
        string bundleName = getBundleName(fileinfo,scenename);
        //BundleName示例： F:/Unity项目/AssetBundlesDemo/Assets/Resource/Scene1/Buildings/Building1.prefab        
        int index = fileinfo.FullName.IndexOf("Assets");
        string assetPath = fileinfo.FullName.Substring(index);
        //assetPath示例： Assets/Resource/Scene1/Buildings/Building1.prefab
        //6.用AssetImproter类 修改名称和后缀
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        //修改assetbundle的labels
        assetImporter.assetBundleName = bundleName.ToLower();
        //修改assetbundle的后缀
        if (fileinfo.Extension == ".unity")
            assetImporter.assetBundleVariant = "u3d";
        else
            assetImporter.assetBundleVariant = "assetbundle";

        //把文件夹名和具体路径的关系，添加到字典里
        //获取文件夹名
        string folderName = "";
        if (bundleName.Contains("/"))
            folderName = bundleName.Split('/')[1];
        else
            folderName = bundleName;
        //获取具体路径名
        string bundlePath = assetImporter.assetBundleName + "." + assetImporter.assetBundleVariant;
        //如果字典中不存在文件夹名和具体路径名的关系，则把关系添加进字典中
        if (!namepathdict.ContainsKey(folderName))
            namepathdict.Add(folderName, bundlePath);
    }

    /// <summary>
    /// 获取包名
    /// </summary>
    /// <param name="fileinfo"></param>
    /// <param name="scenename"></param>
    /// <returns></returns>
    private static string getBundleName(FileInfo fileinfo,string scenename)
    {
        string windowsPath = fileinfo.FullName;
        //转换成Unity可识别的路径
        string unityPath = windowsPath.Replace(@"\", "/");
        //Debug.Log("Unity文件路径 ： "+unityPath);


        //Unity路径示例：F:/Unity项目/AssetBundlesDemo/Assets/Resource/Scene1/Buildings/Building1.prefab
        //Windows路径示例：F:\Unity项目\AssetBundlesDemo\Assets\Resource\Scene1\Buildings\Building1.prefab
        int sceneIndex = unityPath.IndexOf(scenename) + scenename.Length;
        //子串切割后的prefabs示例：/Buildings/Building1.prefab
        //子串切割后的场景示例：Scene1.unity
        string bundlePath = unityPath.Substring(sceneIndex + 1);
        //根据是否含有/，来判断是否是场景资源
        if (bundlePath.Contains("/"))
        {
            //prefabs标签：Scene1/Building1.prefab
            string[] temp = bundlePath.Split('/');
            return scenename + "/" + temp[0];
        }
        else
        {
            //场景标签：Scene1
            return scenename;
        }
    }

    #endregion


    #region assetbundle打包

    [MenuItem("AssetBundle/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        //获取打包的输出路径
        string outPath = PathUtil.GetAssetBundleOutPath();
        //打包
        BuildPipeline.BuildAssetBundles(outPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

        Debug.Log("打包成功");
    }
    #endregion

    #region 删除assetbundle
    [MenuItem("AssetBundle/Delete All AssetBundles")]
    static void DeleteAllAssetBundles()
    {
        //获取打包的输出路径
        string outPath = PathUtil.GetAssetBundleOutPath();
        //删除所有文件，包括.meta
        Directory.Delete(outPath, true);
        File.Delete(outPath + ".meta");
        //刷新Unity界面
        AssetDatabase.Refresh();
        Debug.Log("删除成功");
    }

    #endregion

}
