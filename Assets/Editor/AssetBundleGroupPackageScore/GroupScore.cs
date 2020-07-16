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
            Debug.Log("================分组评分结束================");
        }
    }
}