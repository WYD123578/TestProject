using System.Collections.Generic;
using System.Linq;

namespace AssetBundleGroupPackageScore.Group
{
    internal class GroupDivider
    {
        private readonly Dictionary<int, int> _nodeId2GroupIdDict = new Dictionary<int, int>();

        /// <summary>
        /// 查找一个节点的组号
        /// </summary>
        /// <param name="nodeId">nodeId节点信息</param>
        /// <returns>组号</returns>
        private int FindGroupId(int nodeId)
        {
            InitNode(nodeId);
            //-------如果node组号可以查到有其他组号，那么更新node的组号-------
            var findId = nodeId;
            while (_nodeId2GroupIdDict[findId] != findId)
            {
                _nodeId2GroupIdDict[findId] = _nodeId2GroupIdDict[_nodeId2GroupIdDict[findId]];
                findId = _nodeId2GroupIdDict[findId];
            }

            return findId;
        }

        public void InitNode(int nodeId)
        {
            //-------如果node还未被分配组号，则分配自身为初始组号-------
            if (!_nodeId2GroupIdDict.ContainsKey(nodeId))
                _nodeId2GroupIdDict.Add(nodeId, nodeId);
        }

        /// <summary>
        /// 将一条边的两个节点的组号统一
        /// </summary>
        /// <param name="nodeId1">入度节点</param>
        /// <param name="nodeId2">出度节点</param>
        public void JoinEdgeNode(int nodeId1, int nodeId2)
        {
            var g1 = FindGroupId(nodeId1);
            var g2 = FindGroupId(nodeId2);
            if (g1 < g2)
                _nodeId2GroupIdDict[nodeId2] = g1;
            else if (g1 > g2)
                _nodeId2GroupIdDict[nodeId1] = g2;
        }

        /// <summary>
        /// 输出不同组别的节点构成的图
        /// </summary>
        public List<int>[] OutGraphs()
        {
            var groupDict = new Dictionary<int, List<int>>();

            foreach (var kvp in _nodeId2GroupIdDict)
            {
                var groupId = kvp.Value;
                var nodeId = kvp.Key;
                List<int> groupIds;
                if (!groupDict.TryGetValue(groupId, out groupIds))
                {
                    groupIds = new List<int>();
                    groupDict.Add(groupId, groupIds);
                }

                groupIds.Add(nodeId); //添加ab包序号入组
            }

            return groupDict.Values.ToArray();
        }
    }
}