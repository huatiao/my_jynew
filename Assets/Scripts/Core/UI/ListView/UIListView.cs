using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Collections.Specialized;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ProjectBase.UI
{
    public class UIListView : UIView
    {
        public Transform content;

        public GameObject itemTemplate;

        public object _factory;

        public UIListViewBinder<T> CreateBinder<T>() where T : DIViewModelBase
        {
            var factory = new UIListViewBinder<T>(content, itemTemplate);
            _factory = factory;
            return factory;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_factory != null)
                (_factory as IDisposable)?.Dispose();
        }
    }

    // public class UIListView<T> : UIView where T : DIViewModelBase
    // {
    //     public Transform content;

    //     public GameObject itemTemplate;

    //     public UIListViewBinder<T> CreateBinder()
    //     {
    //         return new UIListViewBinder<T>(content, itemTemplate);
    //     }
    // }
}