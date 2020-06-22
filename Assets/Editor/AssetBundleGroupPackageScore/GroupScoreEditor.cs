using System;
using System.IO;
using System.Text;
using AssetBundleGroupPackageScore.Group;
using AssetBundleGroupPackageScore.Relation;
using AssetBundleGroupPackageScore.Score;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace AssetBundleGroupPackageScore
{
    public class GroupScoreEditor : EditorWindow
    {
        private const string LOCAL_ASSETBUNDLE_PATH_RECORD = "LastAssetBundlePath";
        private const string LOCAL_ASSETBUNDLE_NAME_RECORD = "LastAssetBundleName";
        private const string LOCAL_JSON_SAVE_PATH_RECORD = "LastJsonSavePath";
        private const string LOCAL_IS_SCORED_RECORD = "LastIsScored";

        private string _assetBundlePath;
        private string _assetBundleName;
        private string _jsonSavePath;
        private Vector2 _scrollPos = Vector2.one;

        private bool _isScored;
        private AssetBundleGroup[] _scoredGroups;

        private bool _isBeforePackage;
        private AnimBool _showResultAnim;

        #region 窗口启动

        [MenuItem("Window/AssetGroupScoreWindow")]
        public static void BoostWindow()
        {
            GetWindow<GroupScoreEditor>("AB包分组评分窗口", true).Show();
        }

        #endregion

        private void OnEnable()
        {
            _assetBundlePath = PlayerPrefs.GetString(LOCAL_ASSETBUNDLE_PATH_RECORD);
            _assetBundleName = PlayerPrefs.GetString(LOCAL_ASSETBUNDLE_NAME_RECORD);
            _jsonSavePath = PlayerPrefs.GetString(LOCAL_JSON_SAVE_PATH_RECORD);
            _isScored = PlayerPrefs.GetInt(LOCAL_IS_SCORED_RECORD) == 1;
            if (_assetBundlePath == "") _assetBundlePath = Application.dataPath + "/StreamingAssets";
            if (_assetBundleName == "") _assetBundleName = "StreamingAssets";
            if (_jsonSavePath == "") _jsonSavePath = Application.dataPath + "/result.json";
            if (_isScored)
            {
                if (File.Exists(_jsonSavePath))
                {
                    try
                    {
                        var jsonBytes = File.ReadAllBytes(_jsonSavePath);
                        var jsonStr = Encoding.UTF8.GetString(jsonBytes);
                        _scoredGroups = JsonConvert.DeserializeObject<AssetBundleGroup[]>(jsonStr);
                    }
                    catch (Exception e)
                    {
                        _isScored = false;
                        Debug.LogError(e);
                    }
                }
                else
                {
                    _isScored = false;
                }
            }

            _showResultAnim = new AnimBool(true);
            _showResultAnim.valueChanged.AddListener(Repaint);
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(LOCAL_ASSETBUNDLE_PATH_RECORD, _assetBundlePath);
            PlayerPrefs.SetString(LOCAL_ASSETBUNDLE_NAME_RECORD, _assetBundleName);
            PlayerPrefs.SetString(LOCAL_JSON_SAVE_PATH_RECORD, _jsonSavePath);
            PlayerPrefs.SetInt(LOCAL_IS_SCORED_RECORD, _isScored ? 1 : -1);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            //-----输入路径，名称等参数-----

            var label = _isBeforePackage ? "当前方式为打包前校验" : "当前方式为打包后校验";
            _isBeforePackage = EditorGUILayout.BeginToggleGroup(label, _isBeforePackage);

            var jsonPathReact = EditorGUILayout.GetControlRect();
            var abPathReact = EditorGUILayout.GetControlRect();
            var abNameReact = EditorGUILayout.GetControlRect();
            if (_isBeforePackage)
            {
                _assetBundlePath = EditorGUI.TextField(abPathReact, "AB包所在文件夹路径：", _assetBundlePath);
                _assetBundleName = EditorGUI.TextField(abNameReact, "AB包文件名称：", _assetBundleName);
            }

            EditorGUILayout.EndToggleGroup();

            _jsonSavePath = EditorGUI.TextField(jsonPathReact, "结果Json文件的保存位置：", _jsonSavePath);

            #region -----处理拖拽事件-----

            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragExited))
            {
                //处理AB文件夹路径拖拽问题
                if (abPathReact.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        var path = DragAndDrop.paths[0].Replace('\\', '/');
                        if (Directory.Exists(path))
                        {
                            _assetBundlePath = path;
                        }
                        //如果拖拽的是一个AssetBundle总文件的话
                        else if (File.Exists(path))
                        {
                            var strArr = path.Split('/');
                            if (!strArr[strArr.Length - 1].Contains("."))
                            {
                                _assetBundleName = strArr[strArr.Length - 1];
                                _assetBundlePath = path.Substring(0, path.LastIndexOf('/'));
                            }
                        }
                    }
                }

                //处理AB文件拖拽问题
                if (abNameReact.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        var path = DragAndDrop.paths[0].Replace('\\', '/');
                        if (File.Exists(path))
                        {
                            var strArr = path.Split('/');
                            if (!strArr[strArr.Length - 1].Contains("."))
                            {
                                _assetBundleName = strArr[strArr.Length - 1];
                                _assetBundlePath = path.Substring(0, path.LastIndexOf('/'));
                            }
                            else
                            {
                            }
                        }
                        else if (Directory.Exists(path))
                        {
                            var strArr = path.Split('/');
                            _assetBundlePath = path;
                            _assetBundleName = strArr[strArr.Length - 1];
                        }
                    }
                }

                //处理Json保存路径的问题
                if (jsonPathReact.Contains(Event.current.mousePosition))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                    if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
                    {
                        var path = DragAndDrop.paths[0].Replace('\\', '/');
                        if (File.Exists(path))
                        {
                            _jsonSavePath = path;
                        }
                        else if (Directory.Exists(path))
                        {
                            _jsonSavePath = Path.Combine(path, "result.json");
                        }
                    }
                }
            }

            //-----处理路径是Unity编辑器里面直接拖拽的情况-----
            var unityDataPath = Application.dataPath;
            if (_assetBundlePath.Split('/')[0] == "Assets")
            {
                _assetBundlePath = Path.Combine(unityDataPath, _assetBundleName);
            }

            if (_jsonSavePath.Split('/')[0] == "Assets")
            {
                unityDataPath = unityDataPath.Replace("/Assets", "");
                _jsonSavePath = Path.Combine(unityDataPath, _jsonSavePath);
            }

            #endregion

            //-----参数格式校验-----
            _assetBundlePath = _assetBundlePath.Replace('\\', '/').Trim().TrimEnd('/');
            _assetBundleName = _assetBundleName.Trim();
            _jsonSavePath = _jsonSavePath.Replace('\\', '/').Trim().TrimEnd('/');

            //-----评分-----
            EditorGUILayout.Space(10);
            if (GUILayout.Button("开始分析评分", "LargeButton"))
            {
                try
                {
                    _scoredGroups = GroupDivideAndScore(_jsonSavePath,
                        _isBeforePackage
                            ? NodeRelations.RelationPackaged(_assetBundlePath, _assetBundleName)
                            : new RelationBeforePackage());
                    _isScored = true;
                }
                catch (Exception e)
                {
                    _isScored = false;
                    Debug.LogError(e);
                }
            }
            
            //-----分析------
            if (GUILayout.Button("开始分析关系", "LargeButton"))
            {
                new AssetBundleAnalyse().AnalyseRelation();
            }

            #region -----可视化显示分组评分结果-----

            EditorGUILayout.Space(20);

            _showResultAnim.target =
                EditorGUILayout.BeginFoldoutHeaderGroup(_showResultAnim.target, "分析的结果", null,
                    ShowResultHeaderContextMenu);
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

            if (EditorGUILayout.BeginFadeGroup(_showResultAnim.faded))
            {
                if (_isScored && _scoredGroups != null)
                {
                    EditorGUILayout.LabelField("评分结果如下：");
                    foreach (var scoredGroup in _scoredGroups)
                    {
                        EditorGUILayout.BeginVertical("box");

                        var groupLabel = $"组ID：{scoredGroup.GroupId.ToString()}";
                        var groupScoreLabel = $"组得分：{scoredGroup.GroupScore.ToString()}";
                        var groupNodes = scoredGroup.GroupNodes;
                        EditorGUILayout.LabelField(groupLabel);
                        EditorGUILayout.LabelField(groupScoreLabel);
                        EditorGUILayout.LabelField($"组成员{groupNodes.Length.ToString()}个，具体如下：");

                        for (var i = 0; i < groupNodes.Length; i++)
                        {
                            var groupNode = groupNodes[i];
                            var nodeLabel =
                                $"成员{(i + 1).ToString()}：{groupNode.NodeName}  得分：{groupNode.NodeScore.ToString()}";
                            EditorGUILayout.SelectableLabel(nodeLabel);
                        }

                        if (GUILayout.Button("导出本组成员CSV文件到Json文件所在位置"))
                        {
                            var sb = new StringBuilder();
                            sb.Append("MemberName,MemberScorer\n");
                            foreach (var groupNode in groupNodes)
                            {
                                sb.Append(groupNode.NodeName);
                                sb.Append(',');
                                sb.Append(groupNode.NodeScore.ToString());
                                sb.Append("\r\n");
                            }

                            var path = Path.Combine(_jsonSavePath.Substring(0, _jsonSavePath.LastIndexOf('/')),
                                $"{scoredGroup.GroupId.ToString()}.csv");
                            File.Delete(path);
                            using (var fs = new FileStream(path, FileMode.Create))
                            {
                                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                                fs.Write(bytes, 0, bytes.Length);
                                fs.Close();
                            }
                        }

                        EditorGUILayout.EndVertical();

                        EditorGUILayout.Space(20);
                    }
                }
                else
                {
                    _isScored = false;
                    EditorGUILayout.LabelField("不存在评分结果");
                }
            }

            EditorGUILayout.EndFadeGroup();
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            EditorGUILayout.EndVertical();
        }

        #region json结果窗口功能

        private void ShowResultHeaderContextMenu(Rect menuPosition)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Reset Result"), false, ResetResult);
            menu.AddItem(new GUIContent("Clear Result"), false, ClearResult);
            menu.DropDown(menuPosition);
        }

        private void ResetResult()
        {
            _isScored = false;
            _scoredGroups = null;
        }

        private void ClearResult()
        {
            _isScored = false;
            _scoredGroups = null;
            File.Delete(_jsonSavePath);
        }

        #endregion

        private static AssetBundleGroup[] GroupDivideAndScore(string jsonSavePath, INodeRelation relation)
        {
            AssetBundle.UnloadAllAssetBundles(true);
            var groupScoreManager = new GroupScoreManager();
            var scoreMethod = new GetScoreFromSuffix(
                Path.Combine(Application.dataPath, "Editor/AssetBundleGroupPackageScore/ScoreRules/ScoreMapJson.txt"),
                Path.Combine(Application.dataPath, "Editor/AssetBundleGroupPackageScore/ScoreRules/SuffixMapJson.txt")
            );
            var groups = groupScoreManager.GetAssetGroups(relation);
            for (int i = 0; i < groups.Length; i++)
            {
                groupScoreManager.TopologySortGroup(ref groups[i]);
                groupScoreManager.GetGroupScore(ref groups[i], scoreMethod, relation);
            }

            File.Delete(jsonSavePath);
            using (var fs = new FileStream(jsonSavePath, FileMode.Create))
            {
                var json = JsonConvert.SerializeObject(groups);
                var jsonBytes = Encoding.UTF8.GetBytes(json);
                fs.Write(jsonBytes, 0, jsonBytes.Length);
                fs.Close();
            }
            
            return groups;
        }
    }
}