using AssetBundleGroupPackageScore.Relation;
using UnityEngine;

namespace AssetBundleGroupPackageScore
{
    public static class GroupScore
    {
        private const string PATH = "";
        
        private static readonly GroupScoreManager GroupScoreManager = new GroupScoreManager();

        public static void DivideAndScoreGroup()
        {
            Debug.Log("================开始分析项目AB资源================");
            new AssetBundleAnalyse().AnalyseRelation();
            Debug.Log("================开始划分组别并评分================");
            GroupScoreManager.GetAssetGroups(NodeRelations.RelationBeforePackage());
            
        }
    }
}