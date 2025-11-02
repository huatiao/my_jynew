using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StateSwitcher
{
    /// <summary>
    /// Button属性类型
    /// </summary>
    public enum ButtonPropertyType
    {
        Interactable,
        Enabled
    }

    /// <summary>
    /// Button属性配置
    /// </summary>
    [Serializable]
    public class ButtonPropertyConfig
    {
        public ButtonPropertyType propertyType;
        public bool boolValue = true;
    }

    /// <summary>
    /// Button组件状态处理器
    /// </summary>
    [Serializable]
    public class ButtonStateHandler : IComponentStateHandler
    {
        public List<ButtonPropertyConfig> properties = new List<ButtonPropertyConfig>();

        public void ApplyState(GameObject target)
        {
            if (target == null || properties == null || properties.Count == 0)
            {
                return;
            }

            Button button = target.GetComponent<Button>();
            if (button == null)
            {
                return;
            }

            foreach (var prop in properties)
            {
                switch (prop.propertyType)
                {
                    case ButtonPropertyType.Interactable:
                        button.interactable = prop.boolValue;
                        break;
                        
                    case ButtonPropertyType.Enabled:
                        button.enabled = prop.boolValue;
                        break;
                }
            }
        }
    }
}

