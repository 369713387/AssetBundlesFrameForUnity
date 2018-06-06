using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
/// <summary>
/// 场景里面所有AB包的管理器(把文件夹名 转换为 AB包名)
/// </summary>
public class SceneManager
{
    /// <summary>
    /// 一个场景里面所有的AB包
    /// </summary>
    private OneSceneAssetBundle sceneAB;
    /// <summary>
    /// 文件夹名 和 AB包名的字典映射关系
    /// </summary>
    private Dictionary<string, string> FolderAndBundleDict;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="scenename"></param>
    public SceneManager(string scenename)
    {
        sceneAB = new OneSceneAssetBundle(scenename);
        FolderAndBundleDict = new Dictionary<string, string>();
    }

    /// <summary>
    /// 读取记录文件信息，添加字典映射
    /// </summary>
    /// <param name="scenename"></param>
    public void ReadRecord(string scenename)
    {
        //获取记录文件的路径
        string path = PathUtil.GetAssetBundleOutPath() + "/" + scenename + "Record.config";
        //通过字节流来读取文件里的信息
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                //读取包的数量
                int count = int.Parse(sr.ReadLine());
                //读取每一行的信息,给每个包添加映射关系
                for (int i = 0; i < count; ++i)
                {
                    string line = sr.ReadLine();
                    string[] kv = line.Split('-');
                    FolderAndBundleDict.Add(kv[0], kv[1]);
                }
            }
        }
    }

    /// <summary>
    /// 根据文件夹名，获取完整的AB包名
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public string GetBundleName(string foldername)
    {
        if (!FolderAndBundleDict.ContainsKey(foldername))
        {
            Debug.LogError("没有这个文件夹 ： " + foldername);
            return null;
        }
        return FolderAndBundleDict[foldername];
    }

    #region 集成底层方法

    /// <summary>
    /// 是否加载了指定的AB包
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public bool isLoaded(string foldername)
    {
        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            //如果加载了指定的AB包，返回true
            return sceneAB.isLoaded(abname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的AB包");
            return false;
        }
    }

    /// <summary>
    /// 是否加载完成指定的AB包
    /// </summary>
    /// <param name="foldername"></param>
    /// <returns></returns>
    public bool isLoadCompleted(string foldername)
    {
        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            //如果指定的AB包加载完成，返回true
            return sceneAB.isLoadCompleted(abname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的AB包");
            return false;
        }
    }

    /// <summary>
    /// 根据文件夹名加载AB包
    /// </summary>
    /// <param name="foldername"></param>
    /// <param name="lp"></param>
    /// <param name="labcb"></param>
    public void LoadAssetBundle(string foldername, LoadProgress lp, LoadAssetBundleCallBack labcb)
    {
        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            //开始加载AB包
            sceneAB.LoadAssetBundle(abname, lp, labcb);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的AB包");
        }
    }

    /// <summary>
    /// 加载AB包
    /// </summary>
    /// <param name="abname"></param>
    /// <returns></returns>
    public IEnumerator Load(string abname)
    {
        yield return sceneAB.Load(abname);
    }
    /// <summary>
    /// 根据文件夹名获取单个资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>Objcet对象</returns>
    public Object LoadAsset(string foldername, string assetname)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取该" + assetname + "资源");
            return null;
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            //加载单个资源
            return sceneAB.LoadAsset(abname, assetname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
            return null;
        }
    }

    /// <summary>
    /// 根据文件夹名获取assetbundle包里的所有资源
    /// </summary>
    /// <returns></returns>
    public Object[] LoadAllAssets(string foldername)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取资源");
            return null;
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            return sceneAB.LoadAllAssets(abname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
            return null;
        }
    }

    /// <summary>
    /// 根据文件夹名获取带有子物体的资源
    /// </summary>
    /// <param name="assetname">资源名称</param>
    /// <returns>所有资源</returns>
    public Object[] LoadAssetWithSubAssets(string foldername, string assetname)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取该" + assetname + "资源");
            return null;
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            return sceneAB.LoadAssetWithSubAssets(abname, assetname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
            return null;
        }
    }



    /// <summary>
    /// 根据文件夹名卸载资源
    /// </summary>
    /// <param name="asset">资源</param>
    public void UnLoadAsset(string foldername, string assetname)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取该" + assetname + "资源");
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            sceneAB.UnLoadAsset(abname, assetname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
        }
    }

    /// <summary>
    /// 根据文件夹名卸载一个包里面的所有资源
    /// </summary>
    /// <param name="foldername"></param>
    public void UnLoadAllAsset(string foldername)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取资源");
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            sceneAB.UnLoadAllAsset(abname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
        }
    }

    /// <summary>
    /// 卸载所有包里的所有资源
    /// </summary>
    public void UnLoadAll()
    {
        sceneAB.UnLoadAll();
    }





    /// <summary>
    /// 根据文件夹名卸载内存中的单个AB包
    /// </summary>
    public void Dispose(string foldername)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取资源");
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            sceneAB.Dispose(abname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
        }
    }

    /// <summary>
    /// 卸载所有AB包
    /// </summary>
    public void DisposeAll()
    {
        sceneAB.DisposeAll();
    }

    /// <summary>
    /// 卸载所有的AB包和所有的资源
    /// </summary>
    public void DisposeAllAndUnLoadAllAsset()
    {
        sceneAB.DisposeAllAndUnLoadAllAsset();
    }


    /// <summary>
    /// 查看assetbundle里面所有资源的名称（调试专用）
    /// </summary>
    public void GetAllAssetNames(string foldername)
    {
        if (sceneAB == null)
        {
            Debug.LogError("当前sceneAB为空，无法获取资源名");
        }

        if (FolderAndBundleDict.ContainsKey(foldername))
        {
            //获取包名
            string abname = FolderAndBundleDict[foldername];
            sceneAB.GetAllAssetNames(abname);
        }
        else
        {
            Debug.LogError("找不到这个文件夹" + foldername + "对应的包");
        }
    }
    #endregion
}

