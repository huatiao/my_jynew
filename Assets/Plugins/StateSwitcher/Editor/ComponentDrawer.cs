using UnityEditor;
using UnityEngine;

namespace StateSwitcher.Editor
{
    /// <summary>
    /// 组件配置绘制器
    /// </summary>
    public static class ComponentDrawer
    {
        /// <summary>
        /// 绘制组件配置（统一入口）
        /// </summary>
        public static void DrawComponentConfig(SerializedProperty targetProperty, ComponentType componentType)
        {
            switch (componentType)
            {
                case ComponentType.GameObject:
                    DrawGameObjectConfig(targetProperty);
                    break;
                case ComponentType.Image:
                    DrawImageConfig(targetProperty);
                    break;
                case ComponentType.Button:
                    DrawButtonConfig(targetProperty);
                    break;
                case ComponentType.Text:
                    DrawTextConfig(targetProperty);
                    break;
                    
                case ComponentType.RectTransform:
                    DrawRectTransformConfig(targetProperty);
                    break;
            }
        }

        /// <summary>
        /// 获取Handler属性名称
        /// </summary>
        private static string GetHandlerPropertyName(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.GameObject: return "_gameObjectHandler";
                case ComponentType.Image: return "_imageHandler";
                case ComponentType.Button: return "_buttonHandler";
                case ComponentType.Text: return "_textHandler";
                case ComponentType.RectTransform: return "_rectTransformHandler";
                default: return null;
            }
        }

        /// <summary>
        /// 确保Handler已创建
        /// </summary>
        private static void EnsureHandlerCreated(SerializedProperty targetProperty, ComponentType componentType)
        {
            // 这个方法会在绘制前确保Handler存在
            // 由于Unity序列化限制，实际的创建在UITargetConfig.GetOrCreateHandler中完成
        }

        /// <summary>
        /// 绘制GameObject配置
        /// </summary>
        private static void DrawGameObjectConfig(SerializedProperty targetProperty)
        {
            EnsureHandlerCreated(targetProperty, ComponentType.GameObject);
            
            var handlerProperty = targetProperty.FindPropertyRelative("_gameObjectHandler");
            var propertiesProperty = handlerProperty.FindPropertyRelative("properties");
            
            // 绘制所有属性
            if (propertiesProperty.arraySize > 0)
            {
                for (int i = 0; i < propertiesProperty.arraySize; i++)
                {
                    DrawGameObjectProperty(propertiesProperty, i);
                }
            }
            
            // 添加属性按钮
            if (propertiesProperty.arraySize == 0)
            {
                if (GUILayout.Button("+ Add Property", GUILayout.Height(20)))
                {
                    AddGameObjectProperty(propertiesProperty, GameObjectPropertyType.Active);
                }
            }
        }

        /// <summary>
        /// 绘制单个GameObject属性
        /// </summary>
        private static void DrawGameObjectProperty(SerializedProperty propertiesProperty, int index)
        {
            var propProperty = propertiesProperty.GetArrayElementAtIndex(index);
            var propertyTypeProperty = propProperty.FindPropertyRelative("propertyType");
            var boolValueProperty = propProperty.FindPropertyRelative("boolValue");
            
            GameObjectPropertyType propType = (GameObjectPropertyType)propertyTypeProperty.enumValueIndex;
            
            EditorGUILayout.BeginHorizontal();
            
            // 属性名称（带图标）
            GUIContent labelContent = new GUIContent($"• {propType}");
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(100));
            EditorGUILayout.PropertyField(boolValueProperty, GUIContent.none);
            
            // 删除按钮
            if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(18)))
            {
                propertiesProperty.DeleteArrayElementAtIndex(index);
                propertiesProperty.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 添加GameObject属性
        /// </summary>
        private static void AddGameObjectProperty(SerializedProperty propertiesProperty, GameObjectPropertyType type)
        {
            // 检查是否已存在
            for (int i = 0; i < propertiesProperty.arraySize; i++)
            {
                var prop = propertiesProperty.GetArrayElementAtIndex(i);
                var propType = (GameObjectPropertyType)prop.FindPropertyRelative("propertyType").enumValueIndex;
                if (propType == type)
                {
                    Debug.LogWarning($"属性 '{type}' 已存在，不能重复添加！");
                    return;
                }
            }
            
            propertiesProperty.serializedObject.Update();
            propertiesProperty.InsertArrayElementAtIndex(propertiesProperty.arraySize);
            var newProp = propertiesProperty.GetArrayElementAtIndex(propertiesProperty.arraySize - 1);
            newProp.FindPropertyRelative("propertyType").enumValueIndex = (int)type;
            newProp.FindPropertyRelative("boolValue").boolValue = true;
            propertiesProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制Image配置
        /// </summary>
        private static void DrawImageConfig(SerializedProperty targetProperty)
        {
            EnsureHandlerCreated(targetProperty, ComponentType.Image);
            
            var handlerProperty = targetProperty.FindPropertyRelative("_imageHandler");
            var propertiesProperty = handlerProperty.FindPropertyRelative("properties");
            
            // 绘制所有属性
            if (propertiesProperty.arraySize > 0)
            {
                for (int i = 0; i < propertiesProperty.arraySize; i++)
                {
                    DrawImageProperty(propertiesProperty, i);
                }
            }
            
            // 添加属性按钮
            if (GUILayout.Button("+ Add Property", GUILayout.Height(20)))
            {
                ShowAddImagePropertyMenu(propertiesProperty);
            }
        }

        /// <summary>
        /// 显示添加Image属性菜单
        /// </summary>
        private static void ShowAddImagePropertyMenu(SerializedProperty propertiesProperty)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Sprite"), false, () => AddImageProperty(propertiesProperty, ImagePropertyType.Sprite));
            menu.AddItem(new GUIContent("Color"), false, () => AddImageProperty(propertiesProperty, ImagePropertyType.Color));
            menu.AddItem(new GUIContent("Material"), false, () => AddImageProperty(propertiesProperty, ImagePropertyType.Material));
            menu.AddItem(new GUIContent("Raycast Target"), false, () => AddImageProperty(propertiesProperty, ImagePropertyType.RaycastTarget));
            menu.AddItem(new GUIContent("Enabled"), false, () => AddImageProperty(propertiesProperty, ImagePropertyType.Enabled));
            menu.AddItem(new GUIContent("Set Native Size"), false, () => AddImageProperty(propertiesProperty, ImagePropertyType.SetNativeSize));
            menu.ShowAsContext();
        }

        /// <summary>
        /// 添加Image属性
        /// </summary>
        private static void AddImageProperty(SerializedProperty propertiesProperty, ImagePropertyType type)
        {
            // 检查是否已存在
            for (int i = 0; i < propertiesProperty.arraySize; i++)
            {
                var prop = propertiesProperty.GetArrayElementAtIndex(i);
                var propType = (ImagePropertyType)prop.FindPropertyRelative("propertyType").enumValueIndex;
                if (propType == type)
                {
                    Debug.LogWarning($"属性 '{type}' 已存在，不能重复添加！");
                    return;
                }
            }
            
            propertiesProperty.serializedObject.Update();
            propertiesProperty.InsertArrayElementAtIndex(propertiesProperty.arraySize);
            var newProp = propertiesProperty.GetArrayElementAtIndex(propertiesProperty.arraySize - 1);
            newProp.FindPropertyRelative("propertyType").enumValueIndex = (int)type;
            newProp.FindPropertyRelative("colorValue").colorValue = Color.white;
            newProp.FindPropertyRelative("boolValue").boolValue = true;
            propertiesProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制单个Image属性
        /// </summary>
        private static void DrawImageProperty(SerializedProperty propertiesProperty, int index)
        {
            var propProperty = propertiesProperty.GetArrayElementAtIndex(index);
            var propertyTypeProperty = propProperty.FindPropertyRelative("propertyType");
            
            ImagePropertyType propType = (ImagePropertyType)propertyTypeProperty.enumValueIndex;
            
            EditorGUILayout.BeginHorizontal();
            
            // 属性名称（带图标）
            GUIContent labelContent = new GUIContent($"• {GetPropertyDisplayName(propType)}");
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(100));
            
            // 根据属性类型显示不同的值字段
            switch (propType)
            {
                case ImagePropertyType.Sprite:
                    var spriteValueProperty = propProperty.FindPropertyRelative("spriteValue");
                    EditorGUILayout.PropertyField(spriteValueProperty, GUIContent.none);
                    break;
                    
                case ImagePropertyType.Color:
                    var colorValueProperty = propProperty.FindPropertyRelative("colorValue");
                    EditorGUILayout.PropertyField(colorValueProperty, GUIContent.none);
                    break;
                    
                case ImagePropertyType.Material:
                    var materialValueProperty = propProperty.FindPropertyRelative("materialValue");
                    EditorGUILayout.PropertyField(materialValueProperty, GUIContent.none);
                    break;
                    
                case ImagePropertyType.RaycastTarget:
                case ImagePropertyType.Enabled:
                    var boolValueProperty = propProperty.FindPropertyRelative("boolValue");
                    EditorGUILayout.PropertyField(boolValueProperty, GUIContent.none);
                    break;
                    
                case ImagePropertyType.SetNativeSize:
                    GUIStyle hintStyle = new GUIStyle(EditorStyles.miniLabel);
                    hintStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f);
                    EditorGUILayout.LabelField("(Action)", hintStyle);
                    break;
            }
            
            // 删除按钮
            if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(18)))
            {
                propertiesProperty.DeleteArrayElementAtIndex(index);
                propertiesProperty.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 获取属性显示名称
        /// </summary>
        private static string GetPropertyDisplayName(ImagePropertyType type)
        {
            switch (type)
            {
                case ImagePropertyType.RaycastTarget: return "Raycast";
                case ImagePropertyType.SetNativeSize: return "Native Size";
                default: return type.ToString();
            }
        }

        /// <summary>
        /// 绘制Button配置
        /// </summary>
        private static void DrawButtonConfig(SerializedProperty targetProperty)
        {
            EnsureHandlerCreated(targetProperty, ComponentType.Button);
            
            var handlerProperty = targetProperty.FindPropertyRelative("_buttonHandler");
            var propertiesProperty = handlerProperty.FindPropertyRelative("properties");
            
            // 绘制所有属性
            if (propertiesProperty.arraySize > 0)
            {
                for (int i = 0; i < propertiesProperty.arraySize; i++)
                {
                    DrawButtonProperty(propertiesProperty, i);
                }
            }
            
            // 添加属性按钮
            if (GUILayout.Button("+ Add Property", GUILayout.Height(20)))
            {
                ShowAddButtonPropertyMenu(propertiesProperty);
            }
        }

        /// <summary>
        /// 显示添加Button属性菜单
        /// </summary>
        private static void ShowAddButtonPropertyMenu(SerializedProperty propertiesProperty)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Interactable"), false, () => AddButtonProperty(propertiesProperty, ButtonPropertyType.Interactable));
            menu.AddItem(new GUIContent("Enabled"), false, () => AddButtonProperty(propertiesProperty, ButtonPropertyType.Enabled));
            menu.ShowAsContext();
        }

        /// <summary>
        /// 添加Button属性
        /// </summary>
        private static void AddButtonProperty(SerializedProperty propertiesProperty, ButtonPropertyType type)
        {
            // 检查是否已存在
            for (int i = 0; i < propertiesProperty.arraySize; i++)
            {
                var prop = propertiesProperty.GetArrayElementAtIndex(i);
                var propType = (ButtonPropertyType)prop.FindPropertyRelative("propertyType").enumValueIndex;
                if (propType == type)
                {
                    Debug.LogWarning($"属性 '{type}' 已存在，不能重复添加！");
                    return;
                }
            }
            
            propertiesProperty.serializedObject.Update();
            propertiesProperty.InsertArrayElementAtIndex(propertiesProperty.arraySize);
            var newProp = propertiesProperty.GetArrayElementAtIndex(propertiesProperty.arraySize - 1);
            newProp.FindPropertyRelative("propertyType").enumValueIndex = (int)type;
            newProp.FindPropertyRelative("boolValue").boolValue = true;
            propertiesProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制单个Button属性
        /// </summary>
        private static void DrawButtonProperty(SerializedProperty propertiesProperty, int index)
        {
            var propProperty = propertiesProperty.GetArrayElementAtIndex(index);
            var propertyTypeProperty = propProperty.FindPropertyRelative("propertyType");
            var boolValueProperty = propProperty.FindPropertyRelative("boolValue");
            
            ButtonPropertyType propType = (ButtonPropertyType)propertyTypeProperty.enumValueIndex;
            
            EditorGUILayout.BeginHorizontal();
            
            // 属性名称（带图标）
            GUIContent labelContent = new GUIContent($"• {propType}");
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(100));
            EditorGUILayout.PropertyField(boolValueProperty, GUIContent.none);
            
            // 删除按钮
            if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(18)))
            {
                propertiesProperty.DeleteArrayElementAtIndex(index);
                propertiesProperty.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制Text配置
        /// </summary>
        private static void DrawTextConfig(SerializedProperty targetProperty)
        {
            EnsureHandlerCreated(targetProperty, ComponentType.Text);
            
            var handlerProperty = targetProperty.FindPropertyRelative("_textHandler");
            var propertiesProperty = handlerProperty.FindPropertyRelative("properties");
            
            // 绘制所有属性
            if (propertiesProperty.arraySize > 0)
            {
                for (int i = 0; i < propertiesProperty.arraySize; i++)
                {
                    DrawTextProperty(propertiesProperty, i);
                }
            }
            
            // 添加属性按钮
            if (GUILayout.Button("+ Add Property", GUILayout.Height(20)))
            {
                ShowAddTextPropertyMenu(propertiesProperty);
            }
        }

        /// <summary>
        /// 显示添加Text属性菜单
        /// </summary>
        private static void ShowAddTextPropertyMenu(SerializedProperty propertiesProperty)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Text"), false, () => AddTextProperty(propertiesProperty, TextPropertyType.Text));
            menu.AddItem(new GUIContent("Font Size"), false, () => AddTextProperty(propertiesProperty, TextPropertyType.FontSize));
            menu.AddItem(new GUIContent("Color"), false, () => AddTextProperty(propertiesProperty, TextPropertyType.Color));
            menu.AddItem(new GUIContent("Font Style"), false, () => AddTextProperty(propertiesProperty, TextPropertyType.FontStyle));
            menu.AddItem(new GUIContent("Alignment"), false, () => AddTextProperty(propertiesProperty, TextPropertyType.Alignment));
            menu.AddItem(new GUIContent("Enabled"), false, () => AddTextProperty(propertiesProperty, TextPropertyType.Enabled));
            menu.ShowAsContext();
        }

        /// <summary>
        /// 添加Text属性
        /// </summary>
        private static void AddTextProperty(SerializedProperty propertiesProperty, TextPropertyType type)
        {
            // 检查是否已存在
            for (int i = 0; i < propertiesProperty.arraySize; i++)
            {
                var prop = propertiesProperty.GetArrayElementAtIndex(i);
                var propType = (TextPropertyType)prop.FindPropertyRelative("propertyType").enumValueIndex;
                if (propType == type)
                {
                    Debug.LogWarning($"属性 '{type}' 已存在，不能重复添加！");
                    return;
                }
            }
            
            propertiesProperty.serializedObject.Update();
            propertiesProperty.InsertArrayElementAtIndex(propertiesProperty.arraySize);
            var newProp = propertiesProperty.GetArrayElementAtIndex(propertiesProperty.arraySize - 1);
            newProp.FindPropertyRelative("propertyType").enumValueIndex = (int)type;
            newProp.FindPropertyRelative("colorValue").colorValue = Color.white;
            newProp.FindPropertyRelative("intValue").intValue = 14;
            newProp.FindPropertyRelative("fontStyleValue").enumValueIndex = 0;
            newProp.FindPropertyRelative("alignmentValue").enumValueIndex = 4; // MiddleCenter
            newProp.FindPropertyRelative("boolValue").boolValue = true;
            propertiesProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制单个Text属性
        /// </summary>
        private static void DrawTextProperty(SerializedProperty propertiesProperty, int index)
        {
            var propProperty = propertiesProperty.GetArrayElementAtIndex(index);
            var propertyTypeProperty = propProperty.FindPropertyRelative("propertyType");
            
            TextPropertyType propType = (TextPropertyType)propertyTypeProperty.enumValueIndex;
            
            EditorGUILayout.BeginHorizontal();
            
            // 属性名称
            GUIContent labelContent = new GUIContent($"• {GetTextPropertyDisplayName(propType)}");
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(100));
            
            // 根据属性类型显示不同的值字段
            switch (propType)
            {
                case TextPropertyType.Text:
                    var textValueProperty = propProperty.FindPropertyRelative("textValue");
                    EditorGUILayout.PropertyField(textValueProperty, GUIContent.none);
                    break;
                    
                case TextPropertyType.FontSize:
                    var intValueProperty = propProperty.FindPropertyRelative("intValue");
                    EditorGUILayout.PropertyField(intValueProperty, GUIContent.none);
                    break;
                    
                case TextPropertyType.Color:
                    var colorValueProperty = propProperty.FindPropertyRelative("colorValue");
                    EditorGUILayout.PropertyField(colorValueProperty, GUIContent.none);
                    break;
                    
                case TextPropertyType.FontStyle:
                    var fontStyleProperty = propProperty.FindPropertyRelative("fontStyleValue");
                    EditorGUILayout.PropertyField(fontStyleProperty, GUIContent.none);
                    break;
                    
                case TextPropertyType.Alignment:
                    var alignmentProperty = propProperty.FindPropertyRelative("alignmentValue");
                    EditorGUILayout.PropertyField(alignmentProperty, GUIContent.none);
                    break;
                    
                case TextPropertyType.Enabled:
                    var boolValueProperty = propProperty.FindPropertyRelative("boolValue");
                    EditorGUILayout.PropertyField(boolValueProperty, GUIContent.none);
                    break;
            }
            
            // 删除按钮
            if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(18)))
            {
                propertiesProperty.DeleteArrayElementAtIndex(index);
                propertiesProperty.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 获取Text属性显示名称
        /// </summary>
        private static string GetTextPropertyDisplayName(TextPropertyType type)
        {
            switch (type)
            {
                case TextPropertyType.FontSize: return "Font Size";
                case TextPropertyType.FontStyle: return "Font Style";
                default: return type.ToString();
            }
        }
        
        #region RectTransform配置
        
        /// <summary>
        /// 绘制RectTransform配置
        /// </summary>
        private static void DrawRectTransformConfig(SerializedProperty targetProperty)
        {
            EnsureHandlerCreated(targetProperty, ComponentType.RectTransform);
            
            var handlerProperty = targetProperty.FindPropertyRelative("_rectTransformHandler");
            if (handlerProperty == null) return;
            
            var propertiesProperty = handlerProperty.FindPropertyRelative("properties");
            if (propertiesProperty == null) return;
            
            // 绘制现有属性
            for (int i = 0; i < propertiesProperty.arraySize; i++)
            {
                DrawRectTransformProperty(propertiesProperty, i);
            }
            
            // 添加属性按钮
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("+ Add Property", GUILayout.Width(100)))
            {
                ShowAddRectTransformPropertyMenu(propertiesProperty);
            }
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 显示添加RectTransform属性菜单
        /// </summary>
        private static void ShowAddRectTransformPropertyMenu(SerializedProperty propertiesProperty)
        {
            GenericMenu menu = new GenericMenu();
            
            foreach (RectTransformPropertyType type in System.Enum.GetValues(typeof(RectTransformPropertyType)))
            {
                menu.AddItem(new GUIContent(GetRectTransformPropertyDisplayName(type)), false, () => AddRectTransformProperty(propertiesProperty, type));
            }
            
            menu.ShowAsContext();
        }
        
        /// <summary>
        /// 添加RectTransform属性
        /// </summary>
        private static void AddRectTransformProperty(SerializedProperty propertiesProperty, RectTransformPropertyType type)
        {
            // 检查是否已存在
            for (int i = 0; i < propertiesProperty.arraySize; i++)
            {
                var prop = propertiesProperty.GetArrayElementAtIndex(i);
                var propType = (RectTransformPropertyType)prop.FindPropertyRelative("propertyType").enumValueIndex;
                if (propType == type)
                {
                    Debug.LogWarning($"属性 {type} 已存在");
                    return;
                }
            }
            
            propertiesProperty.serializedObject.Update();
            propertiesProperty.InsertArrayElementAtIndex(propertiesProperty.arraySize);
            var newProp = propertiesProperty.GetArrayElementAtIndex(propertiesProperty.arraySize - 1);
            newProp.FindPropertyRelative("propertyType").enumValueIndex = (int)type;
            
            // 设置默认值
            var vector2Value = newProp.FindPropertyRelative("vector2Value");
            var vector3Value = newProp.FindPropertyRelative("vector3Value");
            
            switch (type)
            {
                case RectTransformPropertyType.LocalScale:
                    vector3Value.vector3Value = Vector3.one;
                    break;
                default:
                    vector2Value.vector2Value = Vector2.zero;
                    vector3Value.vector3Value = Vector3.zero;
                    break;
            }
            
            propertiesProperty.serializedObject.ApplyModifiedProperties();
        }
        
        /// <summary>
        /// 绘制RectTransform属性
        /// </summary>
        private static void DrawRectTransformProperty(SerializedProperty propertiesProperty, int index)
        {
            var propProperty = propertiesProperty.GetArrayElementAtIndex(index);
            var propertyTypeProperty = propProperty.FindPropertyRelative("propertyType");
            var propType = (RectTransformPropertyType)propertyTypeProperty.enumValueIndex;
            
            EditorGUILayout.BeginHorizontal();
            
            // 属性名称
            GUIContent labelContent = new GUIContent($"• {GetRectTransformPropertyDisplayName(propType)}");
            EditorGUILayout.LabelField(labelContent, GUILayout.Width(130));
            
            // 根据类型绘制对应的字段
            bool isVector3 = (propType == RectTransformPropertyType.LocalScale || propType == RectTransformPropertyType.LocalRotation);
            
            if (isVector3)
            {
                var vector3ValueProperty = propProperty.FindPropertyRelative("vector3Value");
                EditorGUILayout.PropertyField(vector3ValueProperty, GUIContent.none);
            }
            else
            {
                var vector2ValueProperty = propProperty.FindPropertyRelative("vector2Value");
                EditorGUILayout.PropertyField(vector2ValueProperty, GUIContent.none);
            }
            
            // 删除按钮
            if (GUILayout.Button("×", GUILayout.Width(20), GUILayout.Height(18)))
            {
                propertiesProperty.DeleteArrayElementAtIndex(index);
                propertiesProperty.serializedObject.ApplyModifiedProperties();
                GUIUtility.ExitGUI();
            }
            
            EditorGUILayout.EndHorizontal();
        }
        
        /// <summary>
        /// 获取RectTransform属性显示名称
        /// </summary>
        private static string GetRectTransformPropertyDisplayName(RectTransformPropertyType type)
        {
            switch (type)
            {
                case RectTransformPropertyType.AnchoredPosition: return "Anchored Position";
                case RectTransformPropertyType.SizeDelta: return "Size Delta";
                case RectTransformPropertyType.AnchorMin: return "Anchor Min";
                case RectTransformPropertyType.AnchorMax: return "Anchor Max";
                case RectTransformPropertyType.LocalScale: return "Local Scale";
                case RectTransformPropertyType.LocalRotation: return "Local Rotation";
                default: return type.ToString();
            }
        }
        
        #endregion
    }
}

