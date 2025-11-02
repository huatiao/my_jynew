using UnityEngine;

namespace StateSwitcher
{
    /// <summary>
    /// 组件状态处理器接口
    /// </summary>
    public interface IComponentStateHandler
    {
        /// <summary>
        /// 应用状态到目标对象
        /// </summary>
        void ApplyState(GameObject target);
    }
}

