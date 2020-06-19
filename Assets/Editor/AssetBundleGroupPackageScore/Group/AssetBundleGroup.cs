using System.Linq;
using Newtonsoft.Json;

namespace AssetBundleGroupPackageScore.Group
{
    public class AssetBundleGroup
    {
        [JsonProperty("Group_Id")] public int GroupId;
        [JsonProperty("Group_Score")] public long GroupScore;
        [JsonProperty("Group_Nodes")] public GroupNode[] GroupNodes;

        public string[] GetGroupMemberNames()
        {
            return GroupNodes.Select(v => v.NodeName).ToArray();
        }
    }
}