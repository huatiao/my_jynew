using System;
using UnityEngine;
using Loxodon.Framework.Views;
using VContainer;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Observables;
using UnityEngine.EventSystems;

namespace ProjectBase.UI
{
    public class SubView : UIView
    {
        public void Create()
        {
            OnCreate();
        }

        public virtual void OnCreate() {}
    }
}