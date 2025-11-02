using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StateSwitcher.Editor
{
    /// <summary>
    /// Target配置绘制器
    /// </summary>
    public static class TargetConfigDrawer
    {
        /// <summary>
        /// 绘制选中的Target的详细配置
        /// </summary>
        public static void DrawSelectedTargetConfig(SerializedProperty element, UnityEngine.Object targetObject)
        {
            if (targetObject == null)
            {
                DrawEmptyTargetHint();
                return;
            }

            GameObject targetGo = EditorUtils.GetGameObjectFromTarget(targetObject);
            if (targetGo == null)
            {
                EditorGUILayout.Space(3);
                EditorGUILayout.HelpBox("无效的目标对象", MessageType.Error);
                return;
            }

            EditorGUILayout.Space(3);
            DrawConfigurationArea(element, targetObject, targetGo);
        }

        /// <summary>
        /// 绘制空Target提示
        /// </summary>
        private static void DrawEmptyTargetHint()
        {
            EditorGUILayout.Space(10);
            GUIStyle emptyStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            emptyStyle.fontSize = 11;
            EditorGUILayout.LabelField("← Select a target to configure", emptyStyle);
            EditorGUILayout.Space(10);
        }

        /// <summary>
        /// 绘制配置区域
        /// </summary>
        private static void DrawConfigurationArea(SerializedProperty element, UnityEngine.Object targetObject, GameObject targetGo)
        {
            // 判断是GameObject还是Component
            bool isGameObject = targetObject is GameObject;
            Component targetComponent = targetObject as Component;
            
            var enabledComponentsProperty = element.FindPropertyRelative("_enabledComponents");
            
            // 自动添加组件类型（如果是Component）
            if (!isGameObject && targetComponent != null)
            {
                AutoAddComponentType(targetComponent, enabledComponentsProperty);
            }
            
            // 组件配置区域
            DrawEnabledComponents(element, enabledComponentsProperty, targetGo, isGameObject);
        }

        /// <summary>
        /// 显示添加组件菜单
        /// </summary>
        private static void ShowAddComponentMenu(GameObject targetGo, SerializedProperty enabledComponentsProperty)
        {
            GenericMenu menu = new GenericMenu();
            
            // GameObject始终可以添加
            menu.AddItem(new GUIContent(typeof(GameObject).Name), false, () => AddComponentType(enabledComponentsProperty, ComponentType.GameObject));
            
            // 只有存在对应组件才能添加
            TryAddComponentMenuItem<Image>(menu, targetGo, enabledComponentsProperty, ComponentType.Image);
            TryAddComponentMenuItem<Button>(menu, targetGo, enabledComponentsProperty, ComponentType.Button);
            TryAddComponentMenuItem<Text>(menu, targetGo, enabledComponentsProperty, ComponentType.Text);
            TryAddComponentMenuItem<RectTransform>(menu, targetGo, enabledComponentsProperty, ComponentType.RectTransform);
            
            menu.ShowAsContext();
        }
        
        /// <summary>
        /// 尝试添加组件菜单项（仅当GameObject有该组件时）
        /// </summary>
        private static void TryAddComponentMenuItem<T>(GenericMenu menu, GameObject targetGo, SerializedProperty enabledComponentsProperty, ComponentType componentType) where T : Component
        {
            if (targetGo.GetComponent<T>() != null)
            {
                menu.AddItem(new GUIContent(typeof(T).Name), false, () => AddComponentType(enabledComponentsProperty, componentType));
            }
        }

        /// <summary>
        /// 添加组件类型
        /// </summary>
        private static void AddComponentType(SerializedProperty enabledComponentsProperty, ComponentType type)
        {
            // 检查是否已存在
            for (int i = 0; i < enabledComponentsProperty.arraySize; i++)
            {
                var enumValue = (ComponentType)enabledComponentsProperty.GetArrayElementAtIndex(i).enumValueIndex;
                if (enumValue == type)
                {
                    Debug.LogWarning($"组件 '{type}' 已存在，不能重复添加！");
                    return;
                }
            }
            
            enabledComponentsProperty.serializedObject.Update();
            
            // 添加组件类型到列表
            enabledComponentsProperty.InsertArrayElementAtIndex(enabledComponentsProperty.arraySize);
            enabledComponentsProperty.GetArrayElementAtIndex(enabledComponentsProperty.arraySize - 1).enumValueIndex = (int)type;
            
            // 初始化对应的Handler
            var targetProperty = enabledComponentsProperty.serializedObject.FindProperty(enabledComponentsProperty.propertyPath.Replace("._enabledComponents", ""));
            InitializeHandler(targetProperty, type);
            
            enabledComponentsProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 初始化Handler
        /// </summary>
        private static void InitializeHandler(SerializedProperty targetProperty, ComponentType type)
        {
            string handlerName = null;
            switch (type)
            {
                case ComponentType.GameObject: handlerName = "_gameObjectHandler"; break;
                case ComponentType.Image: handlerName = "_imageHandler"; break;
                case ComponentType.Button: handlerName = "_buttonHandler"; break;
                case ComponentType.Text: handlerName = "_textHandler"; break;
            }
            
            if (!string.IsNullOrEmpty(handlerName))
            {
                var handlerProperty = targetProperty.FindPropertyRelative(handlerName);
                // 标记为已修改，Unity会在序列化时创建实例
                handlerProperty.isExpanded = true;
            }
        }

        /// <summary>
        /// 自动添加组件类型（当Target是Component时）
        /// </summary>
        private static void AutoAddComponentType(Component targetComponent, SerializedProperty enabledComponentsProperty)
        {
            ComponentType componentType = ComponentType.GameObject;
            
            if (targetComponent is Image)
            {
                componentType = ComponentType.Image;
            }
            else if (targetComponent is Button)
            {
                componentType = ComponentType.Button;
            }
            else if (targetComponent is Text)
            {
                componentType = ComponentType.Text;
            }
            else if (targetComponent is RectTransform)
            {
                componentType = ComponentType.RectTransform;
            }
            
            // 检查是否已添加
            bool alreadyAdded = false;
            for (int i = 0; i < enabledComponentsProperty.arraySize; i++)
            {
                var enumValue = (ComponentType)enabledComponentsProperty.GetArrayElementAtIndex(i).enumValueIndex;
                if (enumValue == componentType)
                {
                    alreadyAdded = true;
                    break;
                }
            }
            
            // 如果还没添加，自动添加
            if (!alreadyAdded)
            {
                enabledComponentsProperty.serializedObject.Update();
                enabledComponentsProperty.InsertArrayElementAtIndex(enabledComponentsProperty.arraySize);
                enabledComponentsProperty.GetArrayElementAtIndex(enabledComponentsProperty.arraySize - 1).enumValueIndex = (int)componentType;
                enabledComponentsProperty.serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// 绘制已启用的组件配置
        /// </summary>
        private static void DrawEnabledComponents(SerializedProperty targetProperty, SerializedProperty enabledComponentsProperty, GameObject targetGo, bool isGameObject)
        {
            if (enabledComponentsProperty.arraySize == 0)
            {
                DrawNoComponentsHint();
            }
            else
            {
                for (int i = 0; i < enabledComponentsProperty.arraySize; i++)
                {
                    var componentType = (ComponentType)enabledComponentsProperty.GetArrayElementAtIndex(i).enumValueIndex;
                    
                    DrawComponentCard(targetProperty, enabledComponentsProperty, componentType, i);
                    
                    if (i < enabledComponentsProperty.arraySize - 1)
                    {
                        EditorGUILayout.Space(3);
                    }
                }
                
                EditorGUILayout.Space(1);
            }
            
            // 在最下方显示添加组件按钮（仅GameObject类型）
            if (isGameObject && targetGo != null)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("+ Add Component"))
                {
                    ShowAddComponentMenu(targetGo, enabledComponentsProperty);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space(1);
            }
        }

        /// <summary>
        /// 绘制无组件提示
        /// </summary>
        private static void DrawNoComponentsHint()
        {
            GUIStyle emptyBoxStyle = new GUIStyle(EditorStyles.helpBox);
            emptyBoxStyle.alignment = TextAnchor.MiddleCenter;
            emptyBoxStyle.padding = new RectOffset(10, 10, 20, 20);
            
            EditorGUILayout.BeginVertical(emptyBoxStyle);
            GUIStyle emptyTextStyle = new GUIStyle(EditorStyles.label);
            emptyTextStyle.alignment = TextAnchor.MiddleCenter;
            emptyTextStyle.normal.textColor = new Color(0.5f, 0.5f, 0.5f);
            EditorGUILayout.LabelField("No components configured", emptyTextStyle);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制组件卡片
        /// </summary>
        private static void DrawComponentCard(SerializedProperty targetProperty, SerializedProperty enabledComponentsProperty, ComponentType componentType, int index)
        {
            // 组件卡片样式
            GUIStyle componentCardStyle = new GUIStyle(EditorStyles.helpBox);
            componentCardStyle.padding = new RectOffset(8, 8, 6, 6);
            
            EditorGUILayout.BeginVertical(componentCardStyle);
            
            // 组件标题栏
            DrawComponentHeader(componentType, enabledComponentsProperty, index);
            
            EditorGUILayout.Space(3);
            
            // 使用统一入口绘制组件配置
            ComponentDrawer.DrawComponentConfig(targetProperty, componentType);
            
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制组件标题栏
        /// </summary>
        private static void DrawComponentHeader(ComponentType componentType, SerializedProperty enabledComponentsProperty, int index)
        {
            EditorGUILayout.BeginHorizontal();
            
            // 根据组件类型显示不同的图标
            GUIContent iconContent = GetComponentIcon(componentType);
            if (iconContent != null && iconContent.image != null)
            {
                GUILayout.Label(iconContent, GUILayout.Width(20), GUILayout.Height(20));
            }
            
            // 组件名称
            GUIStyle componentNameStyle = new GUIStyle(EditorStyles.boldLabel);
            componentNameStyle.fontSize = 11;
            EditorGUILayout.LabelField(componentType.ToString(), componentNameStyle, GUILayout.Width(80));
            
            GUILayout.FlexibleSpace();
            
            // 删除按钮
            Color originalColor = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.6f, 0.6f);
            if (GUILayout.Button("Remove", GUILayout.Width(60), GUILayout.Height(18)))
            {
                if (EditorUtility.DisplayDialog("确认删除", $"确定要删除 {componentType} 组件配置吗？", "删除", "取消"))
                {
                    enabledComponentsProperty.DeleteArrayElementAtIndex(index);
                    enabledComponentsProperty.serializedObject.ApplyModifiedProperties();
                    GUIUtility.ExitGUI();
                }
            }
            GUI.backgroundColor = originalColor;
            
            EditorGUILayout.EndHorizontal();
            
            // 分隔线
            DrawSeparatorLine();
        }
        
        /// <summary>
        /// 获取组件对应的图标
        /// </summary>
        private static GUIContent GetComponentIcon(ComponentType componentType)
        {
            switch (componentType)
            {
                case ComponentType.GameObject:
                    return EditorGUIUtility.IconContent("GameObject Icon");
                case ComponentType.Image:
                    return EditorGUIUtility.IconContent("Image Icon");
                case ComponentType.Button:
                    return EditorGUIUtility.IconContent("d_Button Icon");
                case ComponentType.Text:
                    return EditorGUIUtility.IconContent("Text Icon");
                case ComponentType.RectTransform:
                    return EditorGUIUtility.IconContent("RectTransform Icon");
                default:
                    return EditorGUIUtility.IconContent("d_PrefabVariant Icon");
            }
        }

        /// <summary>
        /// 绘制分隔线
        /// </summary>
        private static void DrawSeparatorLine()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1);
            rect.height = 1;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.2f));
        }
    }
}

