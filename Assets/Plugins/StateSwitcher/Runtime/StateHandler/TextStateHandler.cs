using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StateSwitcher
{
    /// <summary>
    /// Text属性类型
    /// </summary>
    public enum TextPropertyType
    {
        Text,
        FontSize,
        Color,
        FontStyle,
        Alignment,
        Enabled
    }

    /// <summary>
    /// Text属性配置
    /// </summary>
    [Serializable]
    public class TextPropertyConfig
    {
        public TextPropertyType propertyType;
        public string textValue;
        public int intValue;
        public Color colorValue = Color.white;
        public FontStyle fontStyleValue = FontStyle.Normal;
        public TextAnchor alignmentValue = TextAnchor.MiddleCenter;
        public bool boolValue = true;
    }

    /// <summary>
    /// Text组件状态处理器
    /// </summary>
    [Serializable]
    public class TextStateHandler : IComponentStateHandler
    {
        public List<TextPropertyConfig> properties = new List<TextPropertyConfig>();

        public void ApplyState(GameObject target)
        {
            if (target == null || properties == null || properties.Count == 0)
            {
                return;
            }

            Text text = target.GetComponent<Text>();
            if (text == null)
            {
                return;
            }

            foreach (var prop in properties)
            {
                switch (prop.propertyType)
                {
                    case TextPropertyType.Text:
                        if (!string.IsNullOrEmpty(prop.textValue))
                        {
                            text.text = prop.textValue;
                        }
                        break;
                        
                    case TextPropertyType.FontSize:
                        text.fontSize = prop.intValue;
                        break;
                        
                    case TextPropertyType.Color:
                        text.color = prop.colorValue;
                        break;
                        
                    case TextPropertyType.FontStyle:
                        text.fontStyle = prop.fontStyleValue;
                        break;
                        
                    case TextPropertyType.Alignment:
                        text.alignment = prop.alignmentValue;
                        break;
                        
                    case TextPropertyType.Enabled:
                        text.enabled = prop.boolValue;
                        break;
                }
            }
        }
    }
}

