using System.IO;
using AssetBundleGroupPackageScore.Relation;
using AssetBundleGroupPackageScore.Score;
using Newtonsoft.Json;
using UnityEngine;

namespace AssetBundleGroupPackageScore
{
    public static class GroupScore
    {
        private const string PATH = "";

        public static void DivideAndScoreGroup()
        {
            Debug.Log("================开始分析项目AB资源================");
            new AssetBundleAnalyse().AnalyseRelation();

            Debug.Log("================开始划分组别并评分================");
            var relation = NodeRelations.RelationBeforePackage();
            var groupScoreManager = new GroupScoreManager();
            var scoreMethod = new GetScoreFromSuffix(
                Path.Combine(Application.dataPath, "Editor/AssetBundleGroupPackageScore/ScoreRules/ScoreMapJson.txt"),
                Path.Combine(Application.dataPath, "Editor/AssetBundleGroupPackageScore/ScoreRules/SuffixMapJson.txt")
            );
            var groups = groupScoreManager.GetAssetGroups(relation);
            for (int i = 0; i < groups.Length; i++)
            {
                groupScoreManager.TopologySortGroup(ref groups[i]);
                groupScoreManager.GetGroupScore(ref groups[i], scoreMethod, relation);
            }

            LogRecorder.Record("GroupRes", JsonConvert.SerializeObject(groups));
            LogRecorder.Save("GroupRes");

            var divideCount = 0;
#if UNITY_2018_1_OR_NEWER
            Debug.Log($"================开始根据评分划分打包到{divideCount.ToString()}个平台组================");
#else
            Debug.LogFormat("================开始根据评分划分打包到{0}个平台组================", divideCount.ToString());
#endif
            var platformPackageGroup = groupScoreManager.GetPlatformPackageGroup(divideCount);
            LogRecorder.Record("PlatformPackageGroup", JsonConvert.SerializeObject(platformPackageGroup));
            LogRecorder.Save("PlatformPackageGroup");

            Debug.Log("================结束流程================");
        }
    }
}