using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StateSwitcher
{
    /// <summary>
    /// Image属性类型
    /// </summary>
    public enum ImagePropertyType
    {
        Sprite,
        Color,
        Material,
        RaycastTarget,
        Enabled,
        SetNativeSize
    }

    /// <summary>
    /// Image属性配置
    /// </summary>
    [Serializable]
    public class ImagePropertyConfig
    {
        public ImagePropertyType propertyType;
        public Sprite spriteValue;
        public Color colorValue = Color.white;
        public Material materialValue;
        public bool boolValue = true;
    }

    /// <summary>
    /// Image组件状态处理器
    /// </summary>
    [Serializable]
    public class ImageStateHandler : IComponentStateHandler
    {
        public List<ImagePropertyConfig> properties = new List<ImagePropertyConfig>();

        public void ApplyState(GameObject target)
        {
            if (target == null || properties == null || properties.Count == 0)
            {
                return;
            }

            Image image = target.GetComponent<Image>();
            if (image == null)
            {
                return;
            }

            foreach (var prop in properties)
            {
                switch (prop.propertyType)
                {
                    case ImagePropertyType.Sprite:
                        if (prop.spriteValue != null)
                        {
                            image.sprite = prop.spriteValue;
                        }
                        break;
                        
                    case ImagePropertyType.Color:
                        image.color = prop.colorValue;
                        break;
                        
                    case ImagePropertyType.Material:
                        if (prop.materialValue != null)
                        {
                            image.material = prop.materialValue;
                        }
                        break;
                        
                    case ImagePropertyType.RaycastTarget:
                        image.raycastTarget = prop.boolValue;
                        break;
                        
                    case ImagePropertyType.Enabled:
                        image.enabled = prop.boolValue;
                        break;
                        
                    case ImagePropertyType.SetNativeSize:
                        image.SetNativeSize();
                        break;
                }
            }
        }
    }
}

