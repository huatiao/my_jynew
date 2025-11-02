using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using ProjectBase.UI;

namespace ProjectBase.Editor.UI
{
    [CustomEditor(typeof(UIConf))]
    public class UIConfEditor : UnityEditor.Editor
    {
        #region 私有字段
        private SerializedProperty _confListProp;
        private string _searchFilter = "";
        private string _lastSearchFilter = "";
        private List<int> _filteredIndices = new List<int>();
        private bool _showExportSettings = true;
        private Vector2 _scrollPosition;

        // 代码生成器
        private UICodeGenerator _codeGenerator;
        #endregion

        #region Unity回调
        private void OnEnable()
        {
            _confListProp = serializedObject.FindProperty("ConfDataList");

            // 初始化代码生成器
            _codeGenerator = new UICodeGenerator();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制搜索区域
            DrawSearchArea();

            // 绘制导出设置区域
            DrawExportSettingsArea();

            // 绘制列表区域
            DrawListArea();

            // 绘制底部提示
            DrawBottomHints();

            // 检测实时搜索
            CheckRealtimeSearch();

            serializedObject.ApplyModifiedProperties();
        }
        #endregion


        #region 绘制UI
        private void DrawSearchArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("搜索过滤", GUILayout.Width(80));
            _searchFilter = EditorGUILayout.TextField(_searchFilter);

            if (GUILayout.Button("清除", GUILayout.Width(50)))
            {
                _searchFilter = "";
                _lastSearchFilter = "";
                _filteredIndices.Clear();
            }
            EditorGUILayout.EndHorizontal();

            // 显示过滤结果
            if (!string.IsNullOrEmpty(_searchFilter) && _filteredIndices.Count >= 0)
            {
                EditorGUILayout.HelpBox($"找到 {_filteredIndices.Count} 个匹配项", MessageType.Info);
            }

            EditorGUILayout.Space();
        }

        private void DrawExportSettingsArea()
        {
            _showExportSettings = EditorGUILayout.Foldout(_showExportSettings, "导出UIID设置", true, EditorStyles.foldoutHeader);

            if (_showExportSettings)
            {
                EditorGUILayout.BeginVertical("box");

                EditorGUI.BeginChangeCheck();

                _codeGenerator.ExportNamespace = EditorGUILayout.TextField("命名空间", _codeGenerator.ExportNamespace);
                _codeGenerator.ExportClassName = EditorGUILayout.TextField("类名", _codeGenerator.ExportClassName);

                EditorGUILayout.BeginHorizontal();
                _codeGenerator.ExportPath = EditorGUILayout.TextField("输出路径", _codeGenerator.ExportPath);

                if (GUILayout.Button("浏览...", GUILayout.Width(80)))
                {
                    // 使用延迟调用避免在GUI布局期间打开模态对话框
                    EditorApplication.delayCall += () =>
            {
                string path = EditorUtility.SaveFilePanelInProject(
                    "选择UIID文件保存位置",
                            Path.GetFileName(_codeGenerator.ExportPath),
                    "cs",
                            "选择UIID文件保存位置",
                            Path.GetDirectoryName(_codeGenerator.ExportPath));

                if (!string.IsNullOrEmpty(path))
                {
                    _codeGenerator.ExportPath = path;
                    _codeGenerator.SaveSettings();
                    Repaint();
                }
            };
                }
                EditorGUILayout.EndHorizontal();

                if (EditorGUI.EndChangeCheck())
                {
                    _codeGenerator.SaveSettings();
                }

                EditorGUILayout.Space(5);
                
                // 代码生成设置
                EditorGUILayout.Space(10);
                EditorGUILayout.LabelField("代码生成设置", EditorStyles.boldLabel);
                
                _codeGenerator.CodeNamespace = EditorGUILayout.TextField("代码命名空间", _codeGenerator.CodeNamespace);
                
                EditorGUILayout.BeginHorizontal();
                _codeGenerator.ViewPath = EditorGUILayout.TextField("View路径", _codeGenerator.ViewPath);
                if (GUILayout.Button("浏览...", GUILayout.Width(80)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        string path = EditorUtility.OpenFolderPanel("选择View保存路径", _codeGenerator.ViewPath, "");
                        if (!string.IsNullOrEmpty(path))
                        {
                            // 转换为相对路径
                            if (path.StartsWith(Application.dataPath))
                            {
                                path = "Assets" + path.Substring(Application.dataPath.Length);
                            }
                            _codeGenerator.ViewPath = path;
                            _codeGenerator.SaveSettings();
                            Repaint();
                        }
                    };
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                _codeGenerator.ViewModelPath = EditorGUILayout.TextField("ViewModel路径", _codeGenerator.ViewModelPath);
                if (GUILayout.Button("浏览...", GUILayout.Width(80)))
                {
                    EditorApplication.delayCall += () =>
                    {
                        string path = EditorUtility.OpenFolderPanel("选择ViewModel保存路径", _codeGenerator.ViewModelPath, "");
                        if (!string.IsNullOrEmpty(path))
                        {
                            // 转换为相对路径
                            if (path.StartsWith(Application.dataPath))
                            {
                                path = "Assets" + path.Substring(Application.dataPath.Length);
                            }
                            _codeGenerator.ViewModelPath = path;
                            _codeGenerator.SaveSettings();
                            Repaint();
                        }
                    };
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(5);

                // 导出按钮区域
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("导出UIID文件", GUILayout.Height(30)))
                {
                    // 使用延迟调用避免在GUI布局期间打开模态对话框
                    EditorApplication.delayCall += () =>
                    {
                        var dataList = GetUIConfDataList();
                        _codeGenerator.Generate(dataList);
                        Repaint();
                    };
                }

                if (GUILayout.Button("按ID排序", GUILayout.Height(30), GUILayout.Width(100)))
                {
                    // 使用延迟调用避免在GUI布局期间打开模态对话框
                    EditorApplication.delayCall += () =>
                    {
                        SortListByID();
                    };
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();
        }

        private void DrawListArea()
        {
            bool hasFilter = !string.IsNullOrEmpty(_searchFilter) && _filteredIndices.Count >= 0;

            // 列表头部
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("UI配置列表", EditorStyles.boldLabel);

            if (GUILayout.Button("添加", GUILayout.Width(60)))
            {
                AddNewElement();
            }

            EditorGUILayout.EndHorizontal();

            if (hasFilter && _filteredIndices.Count == 0)
            {
                EditorGUILayout.HelpBox("没有找到匹配的UI配置", MessageType.Warning);
                return;
            }

            // 绘制列表
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));

            if (hasFilter && _filteredIndices.Count > 0)
            {
                DrawFilteredElements();
            }
            else
            {
                DrawAllElements();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawBottomHints()
        {
            EditorGUILayout.Space();

            // 统计信息
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField($"总计: {_confListProp.arraySize} 个UI配置", EditorStyles.miniLabel);

            // 检测ID重复
            var duplicates = CheckDuplicateIDs();
            if (duplicates.Count > 0)
            {
                EditorGUILayout.LabelField($"⚠ 发现 {duplicates.Count} 个重复ID", EditorStyles.miniLabel);
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawAllElements()
        {
            var duplicates = CheckDuplicateIDs();

            for (int i = 0; i < _confListProp.arraySize; i++)
            {
                DrawElement(i, duplicates);
            }
        }

        private void DrawFilteredElements()
        {
            var duplicates = CheckDuplicateIDs();

            for (int i = 0; i < _filteredIndices.Count; i++)
            {
                int actualIndex = _filteredIndices[i];
                DrawElement(actualIndex, duplicates);
            }
        }

        private void DrawElement(int index, HashSet<int> duplicates)
        {
            SerializedProperty element = _confListProp.GetArrayElementAtIndex(index);

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();

            // ID字段
            SerializedProperty idProp = element.FindPropertyRelative("id");

            // 如果ID重复，显示警告色
            if (duplicates.Contains(idProp.intValue))
            {
                Color oldColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.yellow;
                EditorGUILayout.LabelField("ID", GUILayout.Width(20));
                EditorGUILayout.PropertyField(idProp, GUIContent.none, GUILayout.Width(50));
                GUI.backgroundColor = oldColor;
            }
            else
            {
                EditorGUILayout.LabelField("ID", GUILayout.Width(20));
                EditorGUILayout.PropertyField(idProp, GUIContent.none, GUILayout.Width(50));
            }

            GUILayout.FlexibleSpace();

            // 添加绿色生成按钮
            Color oldBgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("生成代码", GUILayout.Width(70)))
            {
                int indexToGenerate = index;
                EditorApplication.delayCall += () =>
                {
                    serializedObject.Update();
                    var element = _confListProp.GetArrayElementAtIndex(indexToGenerate);
                    var data = new UIConfData
                    {
                        id = element.FindPropertyRelative("id").intValue,
                        prefabPath = element.FindPropertyRelative("prefabPath").stringValue,
                        description = element.FindPropertyRelative("description").stringValue
                    };
                    _codeGenerator.GenerateViewAndViewModel(data);
                };
            }
            GUI.backgroundColor = oldBgColor;

            // 上移按钮
            EditorGUI.BeginDisabledGroup(index == 0);
            if (GUILayout.Button("↑", GUILayout.Width(25)))
            {
                _confListProp.MoveArrayElement(index, index - 1);
            }
            EditorGUI.EndDisabledGroup();

            // 下移按钮
            EditorGUI.BeginDisabledGroup(index == _confListProp.arraySize - 1);
            if (GUILayout.Button("↓", GUILayout.Width(25)))
            {
                _confListProp.MoveArrayElement(index, index + 1);
            }
            EditorGUI.EndDisabledGroup();

            // 删除按钮
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                int indexToDelete = index;
                // 使用延迟调用避免在GUI布局期间打开模态对话框
                EditorApplication.delayCall += () =>
                {
                    if (EditorUtility.DisplayDialog("确认删除", "确定要删除这个UI配置吗？", "确定", "取消"))
                    {
                        serializedObject.Update();
                        _confListProp.DeleteArrayElementAtIndex(indexToDelete);
                        serializedObject.ApplyModifiedProperties();
                        UpdateFilteredIndices();
                        Repaint();
                    }
                };
            }

            EditorGUILayout.EndHorizontal();

            // 预制体路径字段
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("预制体", GUILayout.Width(60));
            SerializedProperty prefabPathProp = element.FindPropertyRelative("prefabPath");
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPathProp.stringValue);
            GameObject newPrefab = EditorGUILayout.ObjectField(prefab, typeof(GameObject), false) as GameObject;
            if (newPrefab != null && newPrefab != prefab)
            {
                string path = AssetDatabase.GetAssetPath(newPrefab);
                if (!string.IsNullOrEmpty(path))
                {
                    prefabPathProp.stringValue = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            // 描述字段
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("描述", GUILayout.Width(60));
            SerializedProperty descriptionProp = element.FindPropertyRelative("description");
            EditorGUILayout.PropertyField(descriptionProp, GUIContent.none);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }
        #endregion

        #region 列表操作
        private void AddNewElement()
        {
            int newIndex = _confListProp.arraySize;
            _confListProp.InsertArrayElementAtIndex(newIndex);

            // 计算下一个可用ID
            int nextID = GetNextAvailableID();

            SerializedProperty newItem = _confListProp.GetArrayElementAtIndex(newIndex);
            newItem.FindPropertyRelative("id").intValue = nextID;
            newItem.FindPropertyRelative("prefabPath").stringValue = "";
            newItem.FindPropertyRelative("description").stringValue = "";

            serializedObject.ApplyModifiedProperties();

            // 滚动到底部
            _scrollPosition.y = float.MaxValue;
        }
        #endregion

        #region 搜索功能
        private void CheckRealtimeSearch()
        {
            if (_searchFilter != _lastSearchFilter)
            {
                _lastSearchFilter = _searchFilter;
                UpdateFilteredIndices();
                Repaint();
            }
        }

        private void UpdateFilteredIndices()
        {
            _filteredIndices.Clear();

            if (string.IsNullOrEmpty(_searchFilter))
            {
                return;
            }

            string lowerFilter = _searchFilter.ToLower();

            for (int i = 0; i < _confListProp.arraySize; i++)
            {
                var element = _confListProp.GetArrayElementAtIndex(i);
                string description = element.FindPropertyRelative("description").stringValue.ToLower();
                string prefabPath = element.FindPropertyRelative("prefabPath").stringValue.ToLower();
                string id = element.FindPropertyRelative("id").intValue.ToString();

                if (description.Contains(lowerFilter) ||
                    prefabPath.Contains(lowerFilter) ||
                    id.Contains(lowerFilter))
                {
                    _filteredIndices.Add(i);
                }
            }
        }
        #endregion

        #region 辅助方法 - 数据转换
        /// <summary>
        /// 将SerializedProperty转换为UIConfData列表
        /// </summary>
        private List<UIConfData> GetUIConfDataList()
        {
            List<UIConfData> dataList = new List<UIConfData>();

            for (int i = 0; i < _confListProp.arraySize; i++)
            {
                var element = _confListProp.GetArrayElementAtIndex(i);
                dataList.Add(new UIConfData
                {
                    id = element.FindPropertyRelative("id").intValue,
                    prefabPath = element.FindPropertyRelative("prefabPath").stringValue,
                    description = element.FindPropertyRelative("description").stringValue
                });
            }

            return dataList;
        }
        #endregion

        #region 辅助功能
        private int GetNextAvailableID()
        {
            int maxId = 0;
            for (int i = 0; i < _confListProp.arraySize; i++)
            {
                var item = _confListProp.GetArrayElementAtIndex(i);
                int currentId = item.FindPropertyRelative("id").intValue;
                if (currentId > maxId)
                {
                    maxId = currentId;
                }
            }
            return maxId + 1;
        }

        private HashSet<int> CheckDuplicateIDs()
        {
            HashSet<int> duplicates = new HashSet<int>();
            Dictionary<int, int> idCount = new Dictionary<int, int>();

            for (int i = 0; i < _confListProp.arraySize; i++)
            {
                var element = _confListProp.GetArrayElementAtIndex(i);
                int id = element.FindPropertyRelative("id").intValue;

                if (idCount.ContainsKey(id))
                {
                    idCount[id]++;
                    duplicates.Add(id);
                }
                else
                {
                    idCount[id] = 1;
                }
            }

            return duplicates;
        }

        private void SortListByID()
        {
            if (_confListProp.arraySize <= 1)
            {
                EditorUtility.DisplayDialog("提示", "没有需要排序的项目", "确定");
                return;
            }

            serializedObject.Update();

            // 将所有元素读取到临时列表
            List<UIConfData> tempList = new List<UIConfData>();
            for (int i = 0; i < _confListProp.arraySize; i++)
            {
                var element = _confListProp.GetArrayElementAtIndex(i);
                tempList.Add(new UIConfData
                {
                    id = element.FindPropertyRelative("id").intValue,
                    prefabPath = element.FindPropertyRelative("prefabPath").stringValue,
                    description = element.FindPropertyRelative("description").stringValue
                });
            }

            // 排序
            tempList.Sort((a, b) => a.id.CompareTo(b.id));

            // 写回
            for (int i = 0; i < tempList.Count; i++)
            {
                var element = _confListProp.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("id").intValue = tempList[i].id;
                element.FindPropertyRelative("prefabPath").stringValue = tempList[i].prefabPath;
                element.FindPropertyRelative("description").stringValue = tempList[i].description;
            }

            serializedObject.ApplyModifiedProperties();
            EditorUtility.DisplayDialog("排序完成", "已按ID升序排列", "确定");

            Repaint();
        }
        #endregion
    }
}
