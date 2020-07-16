using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AssetBundleGroupPackageScore.Relation
{
    public class AssetBundleAnalyse
    {
        private Dictionary<string, int> _assetWeightDic = new Dictionary<string, int>();

        public void AnalyseRelation()
        {
            var allAssetBundleNamesInProject = AssetDatabase.GetAllAssetBundleNames();
            var unusedAssetBundleNamesInProject = AssetDatabase.GetUnusedAssetBundleNames();
            var usedAssetBundleNamesInProject =
                allAssetBundleNamesInProject.Except(unusedAssetBundleNamesInProject).ToArray();

            //----对所有将被打包为AB包的资源及其依赖资源进行引用权重的分析----
            foreach (var assetBundleName in usedAssetBundleNamesInProject)
            {
                var assetsInAssetBundle = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
                foreach (var asset in assetsInAssetBundle)
                {
                    //----将资源本身列入权重列表----
                    UpdateWeight(asset);
                    //----资源直接依赖的资源入权重列表----
                    var directDependencyAssets = AssetDatabase.GetDependencies(asset, false);
                    foreach (var dependencyAsset in directDependencyAssets)
                    {
                        UpdateWeight(dependencyAsset);
                    }
                }
            }

            //----权重分析完成后，将非AB包资源筛选出来----
            LogRecorder.Record("AssetBundleAnalyse",
                "========分析所有将被打包为AB包的资源，并计算其被引用的次数（权重）========");
            foreach (var kvp in _assetWeightDic)
            {
                var assetName = kvp.Key;
                var assetWeight = kvp.Value;
                var assetBundleName = AssetDatabase.GetImplicitAssetBundleName(assetName);
                var logMsg = string.IsNullOrEmpty(assetBundleName)
                    ? $"—— 不会打进AB包的资源“{assetName}”被引用到的次数为：{assetWeight.ToString()}"
                    : $"== 将被打包进“{assetBundleName}”的资源“{assetName}”被引用到的次数为：{assetWeight.ToString()}";
                Debug.Log(logMsg);
                LogRecorder.Record("AssetBundleAnalyse", logMsg);
            }

            LogRecorder.Save("AssetBundleAnalyse");
        }

        private void UpdateWeight(string key)
        {
            if (string.IsNullOrEmpty(key)) return;
            if (Path.GetExtension(key).ToLower() == ".cs") return;

            if (_assetWeightDic.ContainsKey(key))
            {
                _assetWeightDic[key]++;
            }
            else
            {
                _assetWeightDic.Add(key, 1);
            }
        }
    }
}