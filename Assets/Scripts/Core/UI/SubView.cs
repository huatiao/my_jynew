using System;
using UnityEngine;
using Loxodon.Framework.Views;
using VContainer;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;

namespace ProjectBase.UI
{
    public abstract class SubView : UIView
    {
        protected object _viewModel;

        public void SetViewModel(object viewModel)
        {
            _viewModel = viewModel;
            this.SetDataContext(_viewModel);
        }
    }

    public abstract class DISubView<T> : SubView where T : DIViewModelBase
    {
        public void Create()
        {
            if (_viewModel is DIViewModelBase viewModel)
            {
                viewModel.Init();
                OnViewCreate();
                viewModel.OnViewCreate();
            }
        }

        protected abstract void OnViewCreate();
    }
}