using Newtonsoft.Json;

namespace AssetBundleGroupPackageScore.Group
{
    public class GroupNode
    {
        [JsonIgnore] public readonly int NodeId;
        [JsonProperty("AssetBundleName")] public readonly string NodeName;
        [JsonProperty("NodeScore")] public long NodeScore;
        [JsonProperty("DirectDependencies")] public string[] NextNodeNames;
        [JsonProperty("AllDependencies")] public string[] AllNextNodeNames;

        public GroupNode(string nodeName, int nodeId)
        {
            NodeId = nodeId;
            NodeName = nodeName;
        }
    }
}