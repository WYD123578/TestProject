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

    [MenuItem("AssetBundle/BuildCertain")]
    public static void BuildTest()
    {
        var builds = new AssetBundleBuild();
        builds.assetBundleName = "p2";
        builds.assetBundleVariant = "";
        builds.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle("p2");
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, new AssetBundleBuild[] {builds},
            BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
    }
}