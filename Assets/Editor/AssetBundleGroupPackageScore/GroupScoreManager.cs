using System;
using System.Collections.Generic;
using System.IO;
using AssetBundleGroupPackageScore.Group;
using AssetBundleGroupPackageScore.Relation;
using AssetBundleGroupPackageScore.Score;

namespace AssetBundleGroupPackageScore
{
    public class GroupScoreManager
    {
        private readonly Dictionary<string, GroupNode> _nodeDict = new Dictionary<string, GroupNode>();
        private readonly Dictionary<int, GroupNode> _nodeIdDict = new Dictionary<int, GroupNode>();
        private AssetBundleGroup[] _groups;

        /// <summary>
        /// 得到一个分组结果
        /// </summary>
        /// <returns>AB组的划分结果数组</returns>
        public AssetBundleGroup[] GetAssetGroups(INodeRelation nodeRelation)
        {
            if (_groups != null) return _groups;
            _nodeDict.Clear();
            _nodeIdDict.Clear();

            //------节点化AB并建立邻接关系------
            var groupNodes = nodeRelation.GetRelationGroupNodes();
            foreach (var groupNode in groupNodes)
            {
                _nodeDict.Add(groupNode.NodeName, groupNode);
                _nodeIdDict.Add(groupNode.NodeId, groupNode);
            }

            //------划分组别------
            var divider = new GroupDivider();

            foreach (var node in groupNodes)
            {
                divider.InitNode(node.NodeId);
                foreach (var nextNodeName in node.NextNodeNames)
                {
                    var nextNode = _nodeDict[nextNodeName];
                    divider.JoinEdgeNode(node.NodeId, nextNode.NodeId);
                }
            }

            //------拿到属于一个组的AB包的编号------
            var groupIdLists = divider.OutGraphs();
            _groups = new AssetBundleGroup[groupIdLists.Length];
            for (var i = 0; i < groupIdLists.Length; i++)
            {
                //------将编号对应的节点入组------
                var groupIdList = groupIdLists[i];
                var thisGroupNodes = new GroupNode[groupIdList.Count];
                for (var j = 0; j < groupIdList.Count; j++)
                {
                    thisGroupNodes[j] = _nodeIdDict[groupIdList[j]];
                }

                _groups[i] = new AssetBundleGroup() {GroupNodes = thisGroupNodes, GroupId = i};
            }

            return _groups;
        }

        public long GetAssetBundleScore(string abName, IScoreGetMethod scoreGetMethod, INodeRelation nodeRelation)
        {
            long abScore = 0;

            var assetNames = nodeRelation.GetNodeAssetNames(abName);
            foreach (var assetName in assetNames)
            {
                var assetSize = File.ReadAllBytes(assetName).Length / 1024;
                abScore += scoreGetMethod.GetScoreFromMap(assetName) * assetSize;
            }
            
            return abScore;
        }

        /// <summary>
        /// 对一个组进行评分
        /// </summary>
        /// <param name="group">要进行评分的组</param>
        /// <param name="scoreGetMethod">获取评分的方法</param>
        /// <param name="nodeRelation">节点关系方法</param>
        public long GetGroupScore(ref AssetBundleGroup group, IScoreGetMethod scoreGetMethod, INodeRelation nodeRelation)
        {
            long groupScore = 0;
            foreach (var node in group.GroupNodes)
            {
                node.NodeScore = GetAssetBundleScore(node.NodeName, scoreGetMethod, nodeRelation);
                groupScore += node.NodeScore;
            }

            group.GroupScore = groupScore;

            return groupScore;
        }

        public List<AssetBundleGroup>[] GetPlatformPackageGroup(int plateFormCount)
        {
            plateFormCount = Math.Min(_groups.Length, plateFormCount);
            return ScoreGroupDivide.AverageAssetBundleGroup(_groups, plateFormCount);
        }

        public void TopologySortGroup(ref AssetBundleGroup group)
        {
            GroupTopologySort.TopologySort(ref group, _nodeDict);
        }
    }
}