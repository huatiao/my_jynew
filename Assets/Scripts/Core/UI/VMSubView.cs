using System;
using UnityEngine;
using Loxodon.Framework.Views;
using VContainer;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;

namespace ProjectBase.UI
{
    public abstract class VMSubView<T> : SubView, IVMView where T : DIViewModelBase
    {
        protected T _viewModel;
        public DIViewModelBase ViewModel => _viewModel;

        public void SetViewModel(T viewModel)
        {
            _viewModel = viewModel;
        }

        public override void OnCreate()
        {
            _viewModel.Init();
            this.SetDataContext(_viewModel);

            OnViewCreate();
            _viewModel.OnViewCreate();
        }

        protected abstract void OnViewCreate();
    }
}