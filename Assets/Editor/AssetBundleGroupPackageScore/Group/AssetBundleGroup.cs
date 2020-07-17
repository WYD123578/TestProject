using System;
using System.Linq;

namespace AssetBundleGroupPackageScore.Group
{
    public class AssetBundleGroup
    {
        public int GroupId;
        public long GroupScore;
        public GroupNode[] GroupNodes;

        public string[] GetGroupMemberNames()
        {
            return GroupNodes.Select(v => v.NodeName).ToArray();
        }
    }
}