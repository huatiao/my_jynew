using VContainer;
using VContainer.Unity;
using UnityEngine;
using System;
using Loxodon.Framework.Views;

namespace ProjectBase.UI
{
    public class ViewScope : LifetimeScope
    {
        public Type ViewModelType { set; get; }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register(ViewModelType, Lifetime.Singleton);

            builder.RegisterBuildCallback(container=> {
                container.InjectGameObject(this.gameObject);
            });
        }
    }
}