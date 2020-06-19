using System;
using System.Collections.Generic;
using System.Linq;
using AssetBundleGroupPackageScore.Group;

namespace AssetBundleGroupPackageScore.Score
{
    public struct ScoreGroupNode
    {
        public readonly int Id;
        public readonly long Score;

        public ScoreGroupNode(int id, long score)
        {
            Id = id;
            Score = score;
        }
    }

    /// <summary>
    /// 将评分后的n个带分数的AB组划分为m个打包组
    /// </summary>
    public static class ScoreGroupDivide
    {
        /// <summary>
        /// 将AB包组按评分划分
        /// </summary>
        /// <param name="groups">划分的目标组</param>
        /// <param name="divideGroupNum">需要划分的数量</param>
        /// <returns>一个列表数组，每一个列表记录了被分在一组的AB包group</returns>
        public static List<AssetBundleGroup>[] AverageAssetBundleGroup(AssetBundleGroup[] groups, int divideGroupNum)
        {
            var groupIdMap = groups.ToDictionary(key => key.GroupId, value => value);
            var nodes = new ScoreGroupNode[groups.Length];
            for (var i = 0; i < groups.Length; i++)
            {
                nodes[i] = new ScoreGroupNode(groups[i].GroupId, groups[i].GroupScore);
            }

            var dividedNodeLists = EasyDivideGroup(nodes, divideGroupNum);
            var res = new List<AssetBundleGroup>[divideGroupNum];
            for (int i = 0; i < dividedNodeLists.Length; i++)
            {
                var dividedNodeList = dividedNodeLists[i];
                res[i] = new List<AssetBundleGroup>();
                foreach (var node in dividedNodeList)
                {
                    res[i].Add(groupIdMap[node.Id]);
                }
            }

            return res;
        }

        /// <summary>
        /// 最简单的分组方式，只能得一个正确的解
        /// </summary>
        /// <param name="nodes">节点</param>
        /// <param name="divideGroupNum">要分出的组</param>
        /// <returns></returns>
        public static List<ScoreGroupNode>[] EasyDivideGroup(ScoreGroupNode[] nodes, int divideGroupNum)
        {
            //数组按分数从大到小排序
            Array.Sort(nodes, (left, right) =>
            {
                if (left.Score == right.Score) return 0;
                return left.Score < right.Score ? -1 : 1;
            });
            var res = new List<ScoreGroupNode>[divideGroupNum];

            var groupScores = new long[divideGroupNum]; //用于记录每组当前的总分

            //计算平均值
            long groupTotalScore = 0;
            foreach (var node in nodes)
            {
                groupTotalScore += node.Score;
            }

            var groupAverageScore = groupTotalScore / divideGroupNum;

            var groupIndex = 0; //指示组下标
            var groupDelta = 1;
            //从大到小依次遍历数字，将它们挨个放入数组中
            for (var i = nodes.Length - 1; i >= 0; i--)
            {
                //如果还没有数组，则添加
                if (res[groupIndex] == null) res[groupIndex] = new List<ScoreGroupNode>();

                //如果组的和已经大于了平均值，则不加
                while (groupScores[groupIndex] >= groupAverageScore)
                {
                    groupIndex++;
                    groupDelta = 1;
                }

                res[groupIndex].Add(nodes[i]);
                groupScores[groupIndex] += nodes[i].Score;

                groupIndex += groupDelta;
                if (groupIndex == divideGroupNum)
                {
                    groupIndex = divideGroupNum - 1;
                    groupDelta = -1;
                }

                if (groupIndex < 0)
                {
                    groupIndex = 0;
                    groupDelta = 1;
                }
            }

            return res;
        }
    }
}