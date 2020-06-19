using AssetBundleGroupPackageScore.Group;

namespace AssetBundleGroupPackageScore.Relation
{
    public interface INodeRelation
    {
        /// <summary>
        /// 得到一个构建了邻接关系的节点数组
        /// </summary>
        GroupNode[] GetRelationGroupNodes();
        /// <summary>
        /// 得到属于一个节点的所有资源的路径
        /// </summary>
        /// <param name="nodeName">节点名（AB包名）</param>
        string[] GetNodeAssetNames(string nodeName);
    }
}