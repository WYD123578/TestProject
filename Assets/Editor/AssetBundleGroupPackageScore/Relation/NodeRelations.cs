using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using AssetBundleGroupPackageScore.Group;
using UnityEngine;

namespace AssetBundleGroupPackageScore.Relation
{
    public abstract class NodeRelations
    {
        public static INodeRelation RelationBeforePackage()
        {
            return new RelationBeforePackage();
        }

        public static INodeRelation RelationPackaged(string assetBundlePath, string assetBundleName)
        {
            return new RelationPackaged(assetBundlePath, assetBundleName);
        }
    }

    /// <summary>
    /// 未打包AB前，节点关系的分析处理
    /// </summary>
    internal class RelationBeforePackage : INodeRelation
    {
        public GroupNode[] GetRelationGroupNodes()
        {
            var allAssetBundleNamesInProject = AssetDatabase.GetAllAssetBundleNames();
            var unusedAssetBundleNamesInProject = AssetDatabase.GetUnusedAssetBundleNames();
            var usedAssetBundleNamesInProject = allAssetBundleNamesInProject.Except(unusedAssetBundleNamesInProject).ToArray();

            #region ----Debug一下未使用AB标签的警告----

            var sb = new StringBuilder();
            foreach (var name in unusedAssetBundleNamesInProject)
            {
                sb.Append('[');
                sb.Append(name);
                sb.Append(']');
            }
            Debug.LogWarning($"有{unusedAssetBundleNamesInProject.Length.ToString()}个未使用的标签，分别是：{sb}");

            #endregion
            
            var groupNodes = new GroupNode[usedAssetBundleNamesInProject.Length];
            for (var i = 0; i < usedAssetBundleNamesInProject.Length; i++)
            {
                var assetBundleName = usedAssetBundleNamesInProject[i];
                var abAllDependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, true);
                var abDirectDependencies = AssetDatabase.GetAssetBundleDependencies(assetBundleName, false);
                var groupNode = new GroupNode(assetBundleName, i)
                {
                    NextNodeNames = abDirectDependencies, AllNextNodeNames = abAllDependencies
                };
                groupNodes[i] = groupNode;
            }

            return groupNodes;
        }

        public string[] GetNodeAssetNames(string nodeName)
        {
            return AssetDatabase.GetAssetPathsFromAssetBundle(nodeName);
        }
    }

    /// <summary>
    /// 打包AB之后，节点关系的处理
    /// </summary>
    internal class RelationPackaged : INodeRelation
    {
        private readonly string _assetBundlePath;
        private readonly string _assetBundleName;

        /// <summary>
        /// 打包后的节点关系分析
        /// </summary>
        /// <param name="assetBundlePath">打包后AB的路径</param>
        /// <param name="assetBundleName">打包后总的AB文件的名字</param>
        internal RelationPackaged(string assetBundlePath, string assetBundleName)
        {
            _assetBundlePath = assetBundlePath;
            _assetBundleName = assetBundleName;
        }
        
        public GroupNode[] GetRelationGroupNodes()
        {
            var path = Path.Combine(_assetBundlePath, _assetBundleName);
            var assetBundle = AssetBundle.LoadFromFile(path);
            var manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            
            //------将所有AB包名节点化------
            var abNames = manifest.GetAllAssetBundles();
            var groupNodes = new GroupNode[abNames.Length];
            for (var i = 0; i < abNames.Length; i++)
            {
                //节点id就是其在整体数组中的下标位置
                var nodeName = abNames[i];
                var nodeId = i;
                var node = new GroupNode(nodeName, nodeId)
                {
                    NextNodeNames = manifest.GetDirectDependencies(abNames[i]),//获取直接依赖的包名
                    AllNextNodeNames = manifest.GetAllDependencies(abNames[i])//获取所有依赖的包名
                };
                groupNodes[i] = node;
            }
            
            assetBundle.Unload(true);
            
            return groupNodes;
        }

        public string[] GetNodeAssetNames(string nodeName)
        {
            var assetPath = Path.Combine(_assetBundlePath, nodeName);
            var assetBundle = AssetBundle.LoadFromFile(assetPath);
            var assetNames = assetBundle.GetAllAssetNames();
            var sceneNames = assetBundle.GetAllScenePaths();
            assetBundle.Unload(true);
            AssetBundle.UnloadAllAssetBundles(true);
            return assetNames.Concat(sceneNames).ToArray();
        }
    }
}