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
            foreach (var kvp in _assetWeightDic)
            {
                var assetName = kvp.Key;
                var assetWeight = kvp.Value;
                var assetBundleName = AssetDatabase.GetImplicitAssetBundleName(assetName);
                Debug.Log(string.IsNullOrEmpty(assetBundleName)
                    ? $"引用到未被指定打包的资源：{assetName},权重：{assetWeight.ToString()}"
                    : $"引用到打包包名为{assetBundleName}的资源：{assetName},权重：{assetWeight.ToString()}");
            }
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