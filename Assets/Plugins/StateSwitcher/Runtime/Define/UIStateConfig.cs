using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateSwitcher
{
    /// <summary>
    /// UI状态配置
    /// </summary>
    [Serializable]
    public class UIStateConfig
    {
        [Tooltip("状态ID（用于代码中的enum）")]
        public int StateId;

        [Tooltip("状态名称")]
        public string StateName;

        [Tooltip("目标对象列表")]
        public List<UITargetConfig> Targets = new List<UITargetConfig>();

        /// <summary>
        /// 应用状态
        /// </summary>
        public void ApplyState()
        {
            if (Targets.Count == 0)
            {
                return;
            }

            // 遍历所有目标对象，应用状态
            foreach (var targetConfig in Targets)
            {
                if (targetConfig != null)
                {
                    targetConfig.ApplyState();
                }
            }
        }
    }
}

