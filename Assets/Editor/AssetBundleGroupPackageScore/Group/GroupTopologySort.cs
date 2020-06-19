using System.Collections.Generic;
using JetBrains.Annotations;

namespace AssetBundleGroupPackageScore.Group
{
    internal static class GroupTopologySort
    {
        private static bool[] _isMarked;
        private static Dictionary<string, GroupNode> _nodeDict;
        private static readonly Queue<GroupNode> TopologySortNodes = new Queue<GroupNode>();

        /// <summary>
        /// 对一个组别的ab包进行拓扑排序
        /// </summary>
        /// <param name="group">分好的ab组</param>
        /// <param name="nodeDict">ab包节点总数</param>
        public static void TopologySort([NotNull] ref AssetBundleGroup group, Dictionary<string, GroupNode> nodeDict)
        {
            //-----初始化数据-----
            _isMarked = new bool[nodeDict.Count];
            _nodeDict = nodeDict;
            TopologySortNodes.Clear();
            
            //-----开始排序-----
            var groupNodes = group.GroupNodes;
            foreach (var groupNode in groupNodes)
            {
                if(_isMarked[groupNode.NodeId]) continue;
                DeepFirstSearch(groupNode);
            }
        }

        /// <summary>
        /// 深度优先搜索
        /// </summary>
        /// <param name="searchNode">要进行DFS的节点</param>
        private static void DeepFirstSearch(GroupNode searchNode)
        {
            _isMarked[searchNode.NodeId] = true;
            foreach (var nextNodeName in searchNode.NextNodeNames)
            {
                GroupNode nextNode;
                if (_nodeDict.TryGetValue(nextNodeName, out nextNode))
                {
                    if (!_isMarked[nextNode.NodeId])
                    {
                        DeepFirstSearch(nextNode);
                    }
                }
            }

            TopologySortNodes.Enqueue(searchNode);
        }
    }
}