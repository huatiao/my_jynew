using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateSwitcher
{
    /// <summary>
    /// RectTransform属性类型
    /// </summary>
    public enum RectTransformPropertyType
    {
        AnchoredPosition,
        SizeDelta,
        AnchorMin,
        AnchorMax,
        Pivot,
        LocalScale,
        LocalRotation
    }

    /// <summary>
    /// RectTransform属性配置
    /// </summary>
    [Serializable]
    public class RectTransformPropertyConfig
    {
        public RectTransformPropertyType propertyType;
        public Vector2 vector2Value;
        public Vector3 vector3Value;
    }

    /// <summary>
    /// RectTransform组件状态处理器
    /// </summary>
    [Serializable]
    public class RectTransformStateHandler : IComponentStateHandler
    {
        public List<RectTransformPropertyConfig> properties = new List<RectTransformPropertyConfig>();

        public void ApplyState(GameObject target)
        {
            if (target == null || properties == null || properties.Count == 0)
            {
                return;
            }

            RectTransform rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return;
            }

            foreach (var prop in properties)
            {
                switch (prop.propertyType)
                {
                    case RectTransformPropertyType.AnchoredPosition:
                        rectTransform.anchoredPosition = prop.vector2Value;
                        break;
                        
                    case RectTransformPropertyType.SizeDelta:
                        rectTransform.sizeDelta = prop.vector2Value;
                        break;
                        
                    case RectTransformPropertyType.AnchorMin:
                        rectTransform.anchorMin = prop.vector2Value;
                        break;
                        
                    case RectTransformPropertyType.AnchorMax:
                        rectTransform.anchorMax = prop.vector2Value;
                        break;
                        
                    case RectTransformPropertyType.Pivot:
                        rectTransform.pivot = prop.vector2Value;
                        break;
                        
                    case RectTransformPropertyType.LocalScale:
                        rectTransform.localScale = prop.vector3Value;
                        break;
                        
                    case RectTransformPropertyType.LocalRotation:
                        rectTransform.localRotation = Quaternion.Euler(prop.vector3Value);
                        break;
                }
            }
        }
    }
}

