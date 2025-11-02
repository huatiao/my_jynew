using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using ProjectBase.Define;
using TMPro;

namespace ProjectBase.UI
{
    public class TMPTextView : DISubView<TMPTextViewModel>
    {
        [SerializeField] private TextMeshProUGUI _text;

        protected override void OnViewCreate()
        {
            BindingSet<TMPTextView, TMPTextViewModel> bindingSet = this.CreateBindingSet<TMPTextView, TMPTextViewModel>();
            
            bindingSet.Bind(_text).For(v => v.text).To(vm => vm.Text);

            bindingSet.Build();
        }
    }
}

