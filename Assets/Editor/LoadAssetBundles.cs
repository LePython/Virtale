using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadAssetBundles : MonoBehaviour
{
    AssetBundle loadedAssetBundle;

    [SerializeField]
    private string path;

    private void Start() {
        LoadAssetBundle(path);
    }

    void LoadAssetBundle(string bundleUrl)
    {
        loadedAssetBundle = AssetBundle.LoadFromFile(bundleUrl);

        Debug.Log(loadedAssetBundle == null ? "Failed to load AssetBundle" : "AssetBundle was laoded successfully");
    }
}
