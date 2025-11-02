using System;
using System.Collections.Generic;
using UnityEngine;

namespace StateSwitcher
{
    /// <summary>
    /// 组件类型
    /// </summary>
    public enum ComponentType
    {
        GameObject,  //虽然不是组件，但可视为特殊组件
        Image,
        Button,
        Text,
        RectTransform
    }

    /// <summary>
    /// UI目标配置
    /// </summary>
    [Serializable]
    public class UITargetConfig
    {
        [Tooltip("目标对象（GameObject或Component）")]
        public UnityEngine.Object Target;
        
        [Tooltip("启用的组件类型")]
        [SerializeField] private List<ComponentType> _enabledComponents = new List<ComponentType>();

        // 各组件的状态处理器（仅在需要时创建）
        [SerializeField] private GameObjectStateHandler _gameObjectHandler;
        [SerializeField] private ImageStateHandler _imageHandler;
        [SerializeField] private ButtonStateHandler _buttonHandler;
        [SerializeField] private TextStateHandler _textHandler;
        [SerializeField] private RectTransformStateHandler _rectTransformHandler;

        /// <summary>
        /// 获取目标GameObject
        /// </summary>
        private GameObject GetTargetGameObject()
        {
            if (Target == null)
            {
                return null;
            }

            // 如果是GameObject，直接返回
            GameObject go = Target as GameObject;
            if (go != null)
            {
                return go;
            }

            // 如果是Component，返回其GameObject
            Component comp = Target as Component;
            if (comp != null)
            {
                return comp.gameObject;
            }

            return null;
        }

        /// <summary>
        /// 获取或创建Handler（统一方法）
        /// </summary>
        public IComponentStateHandler GetOrCreateHandler(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.GameObject:
                    return _gameObjectHandler ?? (_gameObjectHandler = new GameObjectStateHandler());
                case ComponentType.Image:
                    return _imageHandler ?? (_imageHandler = new ImageStateHandler());
                case ComponentType.Button:
                    return _buttonHandler ?? (_buttonHandler = new ButtonStateHandler());
                case ComponentType.Text:
                    return _textHandler ?? (_textHandler = new TextStateHandler());
                case ComponentType.RectTransform:
                    return _rectTransformHandler ?? (_rectTransformHandler = new RectTransformStateHandler());
                default:
                    return null;
            }
        }

        /// <summary>
        /// 获取Handler（不创建）
        /// </summary>
        private IComponentStateHandler GetHandler(ComponentType type)
        {
            switch (type)
            {
                case ComponentType.GameObject: return _gameObjectHandler;
                case ComponentType.Image: return _imageHandler;
                case ComponentType.Button: return _buttonHandler;
                case ComponentType.Text: return _textHandler;
                case ComponentType.RectTransform: return _rectTransformHandler;
                default: return null;
            }
        }

        /// <summary>
        /// 应用状态
        /// </summary>
        public void ApplyState()
        {
            GameObject targetGo = GetTargetGameObject();
            if (targetGo == null || _enabledComponents.Count == 0)
            {
                return;
            }

            // 按照enabledComponents列表应用对应的Handler
            foreach (var componentType in _enabledComponents)
            {
                var handler = GetHandler(componentType);
                if (handler != null)
                {
                    handler.ApplyState(targetGo);
                }
            }
        }
    }
}