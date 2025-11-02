using System;
using UnityEngine;
using Loxodon.Framework.Views;
using VContainer;
using Loxodon.Framework.Binding;
using VContainer.Unity;

namespace ProjectBase.UI
{
    public abstract class DIWindow<T> : Window where T : DIViewModelBase
    {
        [Inject] protected T _viewModel;

        protected override void OnCreate(IBundle bundle)
        {
            _viewModel.Init();
            
            this.SetDataContext(_viewModel);

            OnViewCreate();
            _viewModel.OnViewCreate();
        }

        protected abstract void OnViewCreate();
    }
}