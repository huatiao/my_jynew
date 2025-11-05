using VContainer;
using VContainer.Unity;
using UnityEngine;
using System;
using Loxodon.Framework.Views;

namespace ProjectBase.UI
{
    public class VMWindowScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            var vmWindow = GetComponent<IVMView>();
            vmWindow.ViewModel.Configure(builder);

            builder.RegisterBuildCallback(container =>{
                container.Inject(vmWindow.ViewModel);
            });
        }
    }
}