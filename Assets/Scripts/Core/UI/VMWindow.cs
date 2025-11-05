using System;
using UnityEngine;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using VContainer;
using VContainer.Unity;

namespace ProjectBase.UI
{
    public abstract class VMWindow<T> : Window, IVMView where T : DIViewModelBase, new()
    {
        protected T _viewModel = new();
        public DIViewModelBase ViewModel => _viewModel;

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