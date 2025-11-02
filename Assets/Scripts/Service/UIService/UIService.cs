using VContainer;
using VContainer.Unity;
using UnityEngine;
using Loxodon.Framework.Views;
using ProjectBase.UI;
using ProjectBase.Define;
using System.Reflection;
using System.Collections.Generic;
using ProjectBase.Model;
using ProjectBase.Util;
using System;

namespace ProjectBase.Service
{
    public enum UILayer
    {
        Main = 0,
    }

    public class UIService
    {
        [Inject] private LifetimeScope _lifetimeScope;
        [Inject] private IObjectResolver _container;
        [Inject] private IUIViewLocator _viewLocator;
        [Inject] private IAssetService _assetService;
        [Inject] private ConfigModel _confModel;


        private GlobalWindowManagerBase _globalWindowManager;
        private Dictionary<UILayer, WindowContainer> _windowContainers = new();
        private WindowContainer _defaultWindowContainer;

        public void InitService()
        {
            _globalWindowManager = GameObject.FindObjectOfType<GlobalWindowManagerBase>();
            if (_globalWindowManager == null)
            {
                Debug.LogError("GlobalWindowManager is null");
                return;
            }

            // 初始化多层WindowContainer
            var layers = typeof(UILayer).GetEnumValues();
            for (int i = 0; i < layers.Length; i++)
            {
                var layer = (UILayer)layers.GetValue(i);
                string goName = $"[{layer.ToString()}]";
                var winContainer = WindowContainer.Create(_globalWindowManager, goName);
                _windowContainers.Add(layer, winContainer);
            }

            _defaultWindowContainer = _windowContainers[UILayer.Main];
        }

        public T OpenUI<T>() where T : Window
        {
            return OpenUI<T>(_defaultWindowContainer);
        }

        public T OpenUI<T>(WindowContainer winContainer) where T : Window
        {
            return InternalOpenUI<T>(winContainer);
        }

        public void CloseUI<T>(T window) where T : Window
        {
            CloseUI<T>(_defaultWindowContainer, window);
        }

        public void CloseUI<T>(WindowContainer winContainer, T window) where T : Window
        {
            winContainer.Remove(window);
        }

        private UIConfData GetUIConfData<T>() where T : Window
        {
            UIInfoAttribute attribute = typeof(T).GetCustomAttribute<UIInfoAttribute>();
            if (attribute == null)
            {
                Debug.LogError($"UIInfoAttribute is null: {typeof(T)}");
                return null;
            }

            return _confModel.GetUIConfData(attribute.ID);
        }

        private T InternalOpenUI<T>(WindowContainer winContainer) where T : Window
        {
            var uiConf = GetUIConfData<T>();
            if (uiConf == null)
            {
                Debug.LogError($"UIConfData is null: {typeof(T)}");
                return null;
            }

            string realPrefabPath = uiConf.prefabPath.Replace(PathUtil.ROOT_RESOURCES, "");
            T window = _viewLocator.LoadWindow<T>(winContainer, realPrefabPath);
            if (window == null)
            {
                Debug.LogError($"Window is null: {typeof(T)} path: {uiConf.prefabPath}");
                return null;
            }

            // TODO 反射内容优化成缓存
            // 检查是否是 VMWindow 类型
            Type baseType = window.GetType().BaseType;
            if (baseType != null && baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(DIWindow<>))
            {
                window.gameObject.SetActive(false);

                var uiScope = window.gameObject.AddComponent<ViewScope>();
                uiScope.parentReference.Object = _lifetimeScope;

                Type viewModelType = baseType.GetGenericArguments()[0];
                uiScope.ViewModelType = viewModelType;

                window.gameObject.SetActive(true);
            }

            window.Create();
            window.Show();
            return window;
        }
    }
}
