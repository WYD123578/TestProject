using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class AssetBundleBuildEditor
{
    [MenuItem("AssetBundle/Build")]
    public static void BuildAssetBundle()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,
            BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
    }
}
