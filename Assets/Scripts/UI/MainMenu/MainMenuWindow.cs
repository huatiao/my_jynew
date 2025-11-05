using UnityEngine;
using UnityEngine.UI;
using Loxodon.Framework.Views;
using Loxodon.Framework.Binding;
using Loxodon.Framework.Binding.Builder;
using Loxodon.Framework.Interactivity;
using ProjectBase.Define;
using TMPro;
using StateSwitcher;

namespace ProjectBase.UI
{
    [UIInfo(UIID.MAIN_MENU_WINDOW)]
    public class MainMenuWindow : VMWindow<MainMenuViewModel>
    {
        [SerializeField] private UIStateSwitcher _uiStateSwitcher;

        [Header("MenuPage")]
        [SerializeField] private Button _btnStart;
        [SerializeField] private Button _btnContinue;
        [SerializeField] private Button _btnSetting;
        [SerializeField] private Button _btnExit;

        [Header("InputNamePage")]
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private Button _btnInputNameConfirm;
        [SerializeField] private Button _btnInputNameBack;
        
        [Header("RandomPropertyPage")]
        [SerializeField] private Button _btnPropertyYes;
        [SerializeField] private Button _btnPropertyNo;
        [SerializeField] private UIListView _listviewProterty;


        protected override void OnViewCreate()
        {
            BindingSet<MainMenuWindow, MainMenuViewModel> bindingSet = this.CreateBindingSet<MainMenuWindow, MainMenuViewModel>();
            
            bindingSet.Bind().For(v => v.OnInteractDismissed).To(vm => vm.InteractDismissed);
            bindingSet.Bind(_uiStateSwitcher).For(v => v.CurrentState).To(vm => (int)vm.CurrentPage);
            bindingSet.Bind(_listviewProterty.CreateBinder<MainMenu.PropertyItemViewModel>()).For(v => v.Items).To(vm => vm.RandomPropertyItems);

            // MenuPage
            bindingSet.Bind(_btnStart).For(v => v.onClick).To(vm => vm.OnBtnStart);
            bindingSet.Bind(_btnContinue).For(v => v.onClick).To(vm => vm.OnBtnContinue);
            bindingSet.Bind(_btnSetting).For(v => v.onClick).To(vm => vm.OnBtnSetting);
            bindingSet.Bind(_btnExit).For(v => v.onClick).To(vm => vm.OnBtnExit);

            // InputNamePage
            bindingSet.Bind(_inputName).For(v => v.text).To(vm => vm.InputName);
            bindingSet.Bind(_btnInputNameConfirm).For(v => v.onClick).To(vm => vm.OnBtnInputNameConfirm);
            bindingSet.Bind(_btnInputNameBack).For(v => v.onClick).To(vm => vm.OnBtnInputNameBack);

            // RandomPropertyPage
            bindingSet.Bind(_btnPropertyYes).For(v => v.onClick).To(vm => vm.OnBtnPropertyYes);
            bindingSet.Bind(_btnPropertyNo).For(v => v.onClick).To(vm => vm.OnBtnPropertyNo);

            bindingSet.Build();
        }

        private void OnInteractDismissed(object sender, InteractionEventArgs e)
        {
            Dismiss();
        }
    }
}

