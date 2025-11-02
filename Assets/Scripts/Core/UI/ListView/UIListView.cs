using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using Loxodon.Framework.Views;
using System;
using System.Collections.Specialized;
using UnityEngine;
using VContainer;

namespace ProjectBase.UI
{
    public class UIListView : UIView
    {
        [Inject] private IObjectResolver _container;

        private IScopedObjectResolver _scopeContainer;


        public Transform content;

        public GameObject itemTemplate;

        public object _factory;

        public UIListViewFactory<T> CreateFactory<T>() where T : DIViewModelBase
        {
            if (_scopeContainer == null)
            {
                _scopeContainer = _container.CreateScope(builder => {
                    builder.Register(typeof(T), Lifetime.Transient);
                });
            }

            var factory = new UIListViewFactory<T>(content, itemTemplate, _scopeContainer);
            _factory = factory;
            return factory;
        }
    }

    // public class UIListView<T> : UIView where T : DIViewModelBase
    // {
    //     public Transform content;

    //     public GameObject itemTemplate;

    //     public UIListViewFactory<T> CreateFactory()
    //     {
    //         return new UIListViewFactory<T>(content, itemTemplate);
    //     }
    // }
}