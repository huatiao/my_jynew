using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateSwitcher
{
    /// <summary>
    /// UI状态切换器
    /// 通过切换状态来改变UI组件的显示
    /// </summary>
    public class UIStateSwitcher : MonoBehaviour
    {
        [SerializeField]
        private int _currentState;

        [SerializeField]
        private List<UIStateConfig> _states = new List<UIStateConfig>();

        [SerializeField]
        [Tooltip("启用重置流程")]
        private bool _enableResetProcess = false;

        [SerializeField]
        [Tooltip("重置状态ID（每次切换前先应用此状态）")]
        private int _resetStateId = -1;

        private Dictionary<int, UIStateConfig> _stateIdDict;

        public int CurrentState 
        {
            get => _currentState;
            set 
            {
                _currentState = value;
                SwitchState(_currentState);
            }
        }

        public List<UIStateConfig> States => _states;

        private void Awake()
        {
            InitializeStateDictionary();
        }

        /// <summary>
        /// 初始化状态字典
        /// </summary>
        private void InitializeStateDictionary()
        {
            _stateIdDict = new Dictionary<int, UIStateConfig>();
            
            foreach (var state in _states)
            {
                // 按ID索引
                if (!_stateIdDict.ContainsKey(state.StateId))
                {
                    _stateIdDict[state.StateId] = state;
                }
            }
        }

        /// <summary>
        /// 通过状态ID切换状态
        /// </summary>
        /// <param name="StateId">状态ID</param>
        public void SwitchState(int StateId)
        {
            if (_stateIdDict == null)
            {
                InitializeStateDictionary();
            }
            
            if (!_stateIdDict.TryGetValue(StateId, out UIStateConfig config))
            {
                Debug.LogWarning($"UIStateSwitcher: 找不到状态ID '{StateId}'");
                return;
            }

            // 先执行重置流程
            if (_enableResetProcess && _resetStateId >= 0 && _resetStateId != StateId)
            {
                ApplyResetState();
            }

            _currentState = config.StateId;
            config.ApplyState();
        }

        /// <summary>
        /// 应用重置状态
        /// </summary>
        private void ApplyResetState()
        {
            if (_stateIdDict == null)
            {
                InitializeStateDictionary();
            }

            if (_stateIdDict.TryGetValue(_resetStateId, out UIStateConfig resetConfig))
            {
                resetConfig.ApplyState();
            }
            else
            {
                Debug.LogWarning($"UIStateSwitcher: 找不到重置状态ID '{_resetStateId}'");
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 在编辑器中切换状态（用于预览）
        /// </summary>
        public void EditorSwitchState(int StateId)
        {
            SwitchState(StateId);
        }

        /// <summary>
        /// 获取所有状态名称
        /// </summary>
        public List<string> GetAllStateNames()
        {
            List<string> stateNames = new List<string>();
            foreach (var state in _states)
            {
                if (!string.IsNullOrEmpty(state.StateName))
                {
                    stateNames.Add(state.StateName);
                }
            }
            return stateNames;
        }
#endif
    }
}

