using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateSwitcher
{
    /// <summary>
    /// GameObject属性类型
    /// </summary>
    public enum GameObjectPropertyType
    {
        Active
    }

    /// <summary>
    /// GameObject属性配置
    /// </summary>
    [Serializable]
    public class GameObjectPropertyConfig
    {
        public GameObjectPropertyType propertyType;
        public bool boolValue = true;
    }

    /// <summary>
    /// GameObject状态处理器
    /// </summary>
    [Serializable]
    public class GameObjectStateHandler : IComponentStateHandler
    {
        public List<GameObjectPropertyConfig> properties = new List<GameObjectPropertyConfig>();

        public void ApplyState(GameObject target)
        {
            if (target == null || properties == null || properties.Count == 0)
            {
                return;
            }

            foreach (var prop in properties)
            {
                switch (prop.propertyType)
                {
                    case GameObjectPropertyType.Active:
                        target.SetActive(prop.boolValue);
                        break;
                }
            }
        }
    }
}

