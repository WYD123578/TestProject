namespace AssetBundleGroupPackageScore.Group
{
    public class GroupNode
    {
        public readonly int NodeId;
        public readonly string NodeName;
        public long NodeScore;
        public string[] NextNodeNames;
        public string[] AllNextNodeNames;

        public GroupNode(string nodeName, int nodeId)
        {
            NodeId = nodeId;
            NodeName = nodeName;
        }
    }
}