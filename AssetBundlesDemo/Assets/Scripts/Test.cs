using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    void LoadProgress(string abname, float progress)
    {
        Debug.Log(progress + " : " + abname);

        if (progress >= 1.0f && abname == AssetBundleManager.instance.GetBundleName("Scene1", "Buildings")) 
        {
            Instantiate(AssetBundleManager.instance.LoadAsset("Scene1", "Buildings", "Building1"));
        }
    }
	void Start () {
        AssetBundleManager.instance.LoadAssetBundle("Scene1", "Buildings", LoadProgress);
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AssetBundleManager.instance.Dispose("Scene1", "Buildings");
        }
	}
}
