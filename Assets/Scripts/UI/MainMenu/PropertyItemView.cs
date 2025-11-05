using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using ProjectBase.Define;
using TMPro;

namespace ProjectBase.UI.MainMenu
{
    public class PropertyItemView : VMSubView<PropertyItemViewModel>
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private GameObject _itemBg;

        protected override void OnViewCreate()
        {
            BindingSet<PropertyItemView, PropertyItemViewModel> bindingSet = this.CreateBindingSet<PropertyItemView, PropertyItemViewModel>();
            
            bindingSet.Bind(_text).For(v => v.text).To(vm => vm.Text);
            bindingSet.Bind(_itemBg.gameObject).For(v => v.activeSelf).To(vm => vm.ActiveItemBg);

            bindingSet.Build();
        }
    }
}

