using UnityEditor;
using UnityEngine;

namespace StateSwitcher.Editor
{
    [CustomEditor(typeof(UIStateSwitcher))]
    public class UIStateSwitcherEditor : UnityEditor.Editor
    {
        private UIStateSwitcher _switcher;
        private int _selectedStateIndex = 0;
        private SerializedProperty _statesProperty;
        private SerializedProperty _enableResetProcessProperty;
        private SerializedProperty _resetStateIdProperty;
        private string _enumName = "UIState";

        private void OnEnable()
        {
            _switcher = (UIStateSwitcher)target;
            _statesProperty = serializedObject.FindProperty("_states");
            _enableResetProcessProperty = serializedObject.FindProperty("_enableResetProcess");
            _resetStateIdProperty = serializedObject.FindProperty("_resetStateId");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 绘制状态预览
            DrawPreviewSection();
            
            EditorGUILayout.Space(5);
            
            // 绘制生成Enum部分
            DrawGenerateEnumSection();
            
            EditorGUILayout.Space(5);

            // 绘制重置流程配置
            DrawResetProcessSection();
            
            EditorGUILayout.Space(5);

            // 绘制状态列表
            DrawStatesHeader();
            
            int stateCount = _statesProperty.arraySize;
            for (int i = 0; i < stateCount; i++)
            {
                DrawStateConfig(i);
            }

            // 添加状态按钮
            DrawAddStateButton(stateCount);

            serializedObject.ApplyModifiedProperties();
        }

        #region 状态列表绘制

        /// <summary>
        /// 绘制状态列表标题
        /// </summary>
        private void DrawStatesHeader()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("状态列表", EditorStyles.boldLabel);
            
            if (GUILayout.Button("全部展开", GUILayout.Width(80)))
            {
                SetAllStatesExpanded(true);
            }
            if (GUILayout.Button("全部折叠", GUILayout.Width(90)))
            {
                SetAllStatesExpanded(false);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制单个状态配置
        /// </summary>
        private void DrawStateConfig(int index)
        {
            var stateProperty = _statesProperty.GetArrayElementAtIndex(index);
            var stateNameProperty = stateProperty.FindPropertyRelative("StateName");
            var stateIdProperty = stateProperty.FindPropertyRelative("StateId");
            var targetsProperty = stateProperty.FindPropertyRelative("Targets");

            EditorGUILayout.BeginVertical("box");

            // 折叠标题栏
            DrawStateHeader(stateProperty, stateNameProperty, stateIdProperty, targetsProperty, index);

            // 展开内容
            if (stateProperty.isExpanded)
            {
                DrawStateContent(stateProperty, stateNameProperty, targetsProperty);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        /// <summary>
        /// 绘制状态标题栏
        /// </summary>
        private void DrawStateHeader(SerializedProperty stateProperty, SerializedProperty stateNameProperty, 
            SerializedProperty stateIdProperty, SerializedProperty targetsProperty, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            // 折叠按钮 + 状态名称（加粗样式）
            string displayName = string.IsNullOrEmpty(stateNameProperty.stringValue) 
                ? $"状态 {index}" 
                : stateNameProperty.stringValue;
            
            // 创建加粗样式
            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
            
            Rect foldoutRect = GUILayoutUtility.GetRect(80, EditorGUIUtility.singleLineHeight);
            stateProperty.isExpanded = EditorGUI.Foldout(foldoutRect, stateProperty.isExpanded, displayName, true, foldoutStyle);
            
            // 状态ID字段
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("ID", GUILayout.Width(20));
            EditorGUILayout.PropertyField(stateIdProperty, GUIContent.none, GUILayout.Width(40));
            if (EditorGUI.EndChangeCheck())
            {
                CheckDuplicateStateId(index, stateIdProperty.intValue);
            }
            
            // 目标数量显示
            EditorGUILayout.LabelField($"({targetsProperty.arraySize} Targets)", GUILayout.Width(80));
            
            // 删除按钮
            if (GUILayout.Button("Delete", GUILayout.Width(60)))
            {
                _statesProperty.DeleteArrayElementAtIndex(index);
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUIUtility.ExitGUI();
                return;
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制状态内容
        /// </summary>
        private void DrawStateContent(SerializedProperty stateProperty, SerializedProperty stateNameProperty, SerializedProperty targetsProperty)
        {
            // EditorGUI.indentLevel++;

            EditorGUILayout.Space(5);

            // 状态名称（加粗标签）
            EditorGUILayout.BeginHorizontal();
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField("状态名称", labelStyle, GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.PropertyField(stateNameProperty, GUIContent.none);
            EditorGUILayout.EndHorizontal();
                
            EditorGUILayout.Space(2);
            
            // 绘制目标列表
            DrawTargetsList(targetsProperty);

            // EditorGUI.indentLevel--;
        }

        /// <summary>
        /// 绘制添加状态按钮
        /// </summary>
        private void DrawAddStateButton(int stateCount)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("+ Add State", GUILayout.Width(100)))
            {
                int newId = GetNextAvailableId();
                
                _statesProperty.InsertArrayElementAtIndex(stateCount);
                var newState = _statesProperty.GetArrayElementAtIndex(stateCount);
                ResetStateConfig(newState);
                
                newState.FindPropertyRelative("StateId").intValue = newId;
                newState.isExpanded = true;
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Targets列表

        /// <summary>
        /// 绘制目标列表
        /// </summary>
        private void DrawTargetsList(SerializedProperty targetsProperty)
        {
            // 默认添加一个空Target
            if (targetsProperty.arraySize == 0)
            {
                targetsProperty.InsertArrayElementAtIndex(0);
                var newTarget = targetsProperty.GetArrayElementAtIndex(0);
                ResetTargetConfig(newTarget);
            }
            
            // 手动绘制列表头部
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Targets", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            
            // 全部展开按钮
            if (GUILayout.Button("[展开]", GUILayout.Width(40)))
            {
                SetAllTargetsExpanded(targetsProperty, true);
            }
            
            // 全部折叠按钮
            if (GUILayout.Button("[折叠]", GUILayout.Width(40)))
            {
                SetAllTargetsExpanded(targetsProperty, false);
            }
            
            // 添加Target按钮
            if (GUILayout.Button("+", GUILayout.Width(30)))
            {
                var index = targetsProperty.arraySize;
                targetsProperty.InsertArrayElementAtIndex(index);
                var newTarget = targetsProperty.GetArrayElementAtIndex(index);
                ResetTargetConfig(newTarget);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.Space(2);
            
            // 绘制每个Target
            for (int i = 0; i < targetsProperty.arraySize; i++)
            {
                DrawTargetElement(targetsProperty, i);
            }
        }
        
        /// <summary>
        /// 设置所有Target的展开/折叠状态
        /// </summary>
        private void SetAllTargetsExpanded(SerializedProperty targetsProperty, bool expanded)
        {
            for (int i = 0; i < targetsProperty.arraySize; i++)
            {
                var element = targetsProperty.GetArrayElementAtIndex(i);
                element.isExpanded = expanded;
            }
        }

        /// <summary>
        /// 绘制单个Target元素
        /// </summary>
        private void DrawTargetElement(SerializedProperty targetsProperty, int index)
        {
            var element = targetsProperty.GetArrayElementAtIndex(index);
            var targetObjectProperty = element.FindPropertyRelative("Target");
            
            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox);
            boxStyle.padding = new RectOffset(6, 6, 4, 4);
            
            EditorGUILayout.BeginVertical(boxStyle);
            
            // 标题栏：折叠箭头 + 对象引用 + 上下移动按钮 + 删除按钮
            EditorGUILayout.BeginHorizontal();
            
            // 折叠箭头（使用固定宽度区域）
            Rect foldoutRect = EditorGUILayout.GetControlRect(GUILayout.Width(15));
            element.isExpanded = EditorGUI.Foldout(foldoutRect, element.isExpanded, GUIContent.none, true);
            
            // 对象引用
            Rect objectFieldRect = EditorGUILayout.GetControlRect();
            
            // 检测鼠标点击事件
            if (Event.current.type == EventType.MouseDown && objectFieldRect.Contains(Event.current.mousePosition))
            {
                // 点击对象引用字段时，展开当前Target，折叠其他Target
                for (int i = 0; i < targetsProperty.arraySize; i++)
                {
                    var otherElement = targetsProperty.GetArrayElementAtIndex(i);
                    otherElement.isExpanded = (i == index);
                }
            }
            
            EditorGUI.PropertyField(objectFieldRect, targetObjectProperty, GUIContent.none);
            
            // 上移按钮
            GUI.enabled = index > 0;
            if (GUILayout.Button("↑", GUILayout.Width(25)))
            {
                targetsProperty.MoveArrayElement(index, index - 1);
            }
            GUI.enabled = true;
            
            // 下移按钮
            GUI.enabled = index < targetsProperty.arraySize - 1;
            if (GUILayout.Button("↓", GUILayout.Width(25)))
            {
                targetsProperty.MoveArrayElement(index, index + 1);
            }
            GUI.enabled = true;
            
            // 删除按钮
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
            if (GUILayout.Button("×", GUILayout.Width(25)))
            {
                targetsProperty.DeleteArrayElementAtIndex(index);
                targetsProperty.serializedObject.ApplyModifiedProperties();
                GUI.backgroundColor = originalColor;
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                GUIUtility.ExitGUI();
                return;
            }
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.EndHorizontal();
            
            // 如果展开，显示详细配置
            if (element.isExpanded)
            {
                EditorGUILayout.Space(3);
                TargetConfigDrawer.DrawSelectedTargetConfig(element, targetObjectProperty.objectReferenceValue);
            }
            
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region 预览和代码生成

        /// <summary>
        /// 绘制重置流程配置
        /// </summary>
        private void DrawResetProcessSection()
        {
            EditorGUILayout.LabelField("重置状态", EditorStyles.boldLabel);

            // 启用重置流程复选框
            EditorGUILayout.PropertyField(_enableResetProcessProperty, new GUIContent("启用"));
            
            // 如果启用了，显示重置状态选择
            if (_enableResetProcessProperty.boolValue)
            {
                var states = _switcher.States;
                if (states != null && states.Count > 0)
                {
                    // 构建状态选项
                    string[] stateOptions = new string[states.Count];
                    int[] stateIds = new int[states.Count];
                    int selectedIndex = 0;
                    
                    for (int i = 0; i < states.Count; i++)
                    {
                        stateIds[i] = states[i].StateId;
                        stateOptions[i] = string.IsNullOrEmpty(states[i].StateName) 
                            ? $"State {i}" 
                            : $"{states[i].StateName}";
                        
                        if (states[i].StateId == _resetStateIdProperty.intValue)
                        {
                            selectedIndex = i;
                        }
                    }
                    
                    // 重置状态下拉菜单
                    EditorGUI.BeginChangeCheck();
                    selectedIndex = EditorGUILayout.Popup("重置状态", selectedIndex, stateOptions);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _resetStateIdProperty.intValue = stateIds[selectedIndex];
                    }
                    
                    // 小提示
                    GUIStyle hintStyle = new GUIStyle(EditorStyles.miniLabel);
                    hintStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
                    hintStyle.wordWrap = true;
                    EditorGUILayout.LabelField("每次切换状态前会先应用重置状态", hintStyle);
                }
                else
                {
                    EditorGUILayout.LabelField("请先添加状态配置", EditorStyles.miniLabel);
                }
            }
        }

        /// <summary>
        /// 绘制预览部分
        /// </summary>
        private void DrawPreviewSection()
        {
            EditorGUILayout.LabelField("状态预览", EditorStyles.boldLabel);

            var states = _switcher.States;

            if (states != null && states.Count > 0)
            {
                // 构建状态选项
                string[] stateOptions = new string[states.Count];
                int[] stateIds = new int[states.Count];
                
                for (int i = 0; i < states.Count; i++)
                {
                    stateIds[i] = states[i].StateId;
                    stateOptions[i] = string.IsNullOrEmpty(states[i].StateName) 
                        ? $"State {i}" 
                        : $"{states[i].StateName}";
                }
                
                EditorGUILayout.BeginHorizontal();
                _selectedStateIndex = Mathf.Clamp(_selectedStateIndex, 0, states.Count - 1);
                _selectedStateIndex = EditorGUILayout.Popup("选择状态", _selectedStateIndex, stateOptions);

                if (GUILayout.Button("预览状态", GUILayout.Width(80)))
                {
                    if (_selectedStateIndex >= 0 && _selectedStateIndex < states.Count)
                    {
                        _switcher.EditorSwitchState(stateIds[_selectedStateIndex]);
                        EditorUtility.SetDirty(_switcher);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("请先添加至少一个状态配置", MessageType.Warning);
            }
        }

        /// <summary>
        /// 绘制生成Enum部分
        /// </summary>
        private void DrawGenerateEnumSection()
        {
            EditorGUILayout.LabelField("代码生成", EditorStyles.boldLabel);

            var stateNames = _switcher.GetAllStateNames();
            
            if (stateNames.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                _enumName = EditorGUILayout.TextField("Enum名称", _enumName);
                
                if (GUILayout.Button("生成并复制Enum", GUILayout.Width(120)))
                {
                    GenerateAndCopyEnum();
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.HelpBox("请先添加至少一个状态再生成Enum", MessageType.Info);
            }
        }

        /// <summary>
        /// 生成Enum代码并复制到剪贴板
        /// </summary>
        private void GenerateAndCopyEnum()
        {
            if (string.IsNullOrEmpty(_enumName))
            {
                EditorUtility.DisplayDialog("错误", "请输入Enum名称", "确定");
                return;
            }
            
            var states = _switcher.States;
            if (states == null || states.Count == 0)
            {
                EditorUtility.DisplayDialog("错误", "没有可用的状态", "确定");
                return;
            }
            
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"public enum {_enumName}");
            sb.AppendLine("{");
            
            for (int i = 0; i < states.Count; i++)
            {
                var state = states[i];
                string StateName = string.IsNullOrEmpty(state.StateName) ? $"State{i}" : state.StateName;
                StateName = EditorUtils.MakeValidIdentifier(StateName);
                
                sb.Append($"    {StateName} = {state.StateId}");
                
                if (i < states.Count - 1)
                {
                    sb.AppendLine(",");
                }
                else
                {
                    sb.AppendLine();
                }
            }
            
            sb.AppendLine("}");
            
            string enumCode = sb.ToString();
            GUIUtility.systemCopyBuffer = enumCode;
            
            Debug.Log($"Enum代码已复制到剪贴板:\n{enumCode}");
            EditorUtility.DisplayDialog("成功", $"Enum '{_enumName}' 已生成并复制到剪贴板！\n\n可以直接粘贴到代码中使用。", "确定");
        }

        #endregion

        #region 工具方法

        /// <summary>
        /// 设置所有状态的展开/折叠状态
        /// </summary>
        private void SetAllStatesExpanded(bool expanded)
        {
            int count = _statesProperty.arraySize;
            for (int i = 0; i < count; i++)
            {
                var stateProperty = _statesProperty.GetArrayElementAtIndex(i);
                stateProperty.isExpanded = expanded;
            }
        }

        /// <summary>
        /// 检查状态ID是否重复
        /// </summary>
        private void CheckDuplicateStateId(int currentIndex, int StateId)
        {
            int count = _statesProperty.arraySize;
            for (int i = 0; i < count; i++)
            {
                if (i == currentIndex)
                    continue;

                var otherState = _statesProperty.GetArrayElementAtIndex(i);
                var otherIdProperty = otherState.FindPropertyRelative("StateId");
                
                if (otherIdProperty.intValue == StateId)
                {
                    Debug.LogWarning($"UIStateSwitcher: Duplicate state ID {StateId}! Please use unique ID values.");
                    break;
                }
            }
        }

        /// <summary>
        /// 获取下一个可用的状态ID
        /// </summary>
        private int GetNextAvailableId()
        {
            int maxId = -1;
            int count = _statesProperty.arraySize;
            
            for (int i = 0; i < count; i++)
            {
                var stateProperty = _statesProperty.GetArrayElementAtIndex(i);
                var idProperty = stateProperty.FindPropertyRelative("StateId");
                
                if (idProperty.intValue > maxId)
                {
                    maxId = idProperty.intValue;
                }
            }
            
            return maxId + 1;
        }

        /// <summary>
        /// 重置状态配置
        /// </summary>
        private void ResetStateConfig(SerializedProperty stateProperty)
        {
            stateProperty.FindPropertyRelative("StateId").intValue = 0;
            stateProperty.FindPropertyRelative("StateName").stringValue = "";
            
            var targetsProperty = stateProperty.FindPropertyRelative("Targets");
            targetsProperty.ClearArray();
        }

        /// <summary>
        /// 重置目标配置
        /// </summary>
        private void ResetTargetConfig(SerializedProperty targetProperty)
        {
            targetProperty.FindPropertyRelative("Target").objectReferenceValue = null;
            targetProperty.isExpanded = true;
            
            // 只清空启用的组件列表，Handler会按需创建
            targetProperty.FindPropertyRelative("_enabledComponents").ClearArray();
        }

        #endregion
    }
}

