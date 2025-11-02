using UnityEngine;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using VContainer;
using ProjectBase.Model;

namespace ProjectBase.UI
{
    public class MainMenuViewModel : DIViewModelBase
    {
        public enum Page
        {
            Reset = 0,
            Menu = 1,
            InputName = 2,
            RandomProperty = 3
        }

        [Inject] private GameRuntimeModel _gameRuntimeModel;


        private InteractionRequest _interactDismissed;
        public InteractionRequest InteractDismissed => _interactDismissed;

        private Page _currentPage = Page.Menu;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set { Set(ref _currentPage, value); }
        }

        private string _inputName;
        public string InputName
        {
            get { return _inputName; }
            set { Set(ref _inputName, value); }
        }

        private ObservableList<TMPTextViewModel> _randomPropertyItems;
        public ObservableList<TMPTextViewModel> RandomPropertyItems
        {
            get { return _randomPropertyItems; }
            set { Set(ref _randomPropertyItems, value); }
        }

        public override void Init()
        {
            _interactDismissed = new InteractionRequest();
        }

        #region MenuPage
        public void OnBtnStart()
        {
            NewGame();
        }

        public void OnBtnContinue()
        {
        }

        public void OnBtnSetting()
        {
        }

        public void OnBtnExit()
        {
            OnBtnInputNameConfirm();
        }

        private void NewGame()
        {
            _gameRuntimeModel.InitNewGame();

            CurrentPage = Page.InputName;
        }
        #endregion

        #region InputNamePage
        public void OnBtnInputNameConfirm()
        {
            CurrentPage = Page.RandomProperty;
            ShowRandomProperty();
        }

        public void OnBtnInputNameBack()
        {
            CurrentPage = Page.Menu;
        }
        #endregion

        #region RandomPropertyPage
        public void OnBtnPropertyYes()
        {
        }

        public void OnBtnPropertyNo()
        {
            CurrentPage = Page.Menu;
        }

        private void ShowRandomProperty()
        {
            var items = new ObservableList<TMPTextViewModel>();
            for (int i = 0; i < 10; i++)
            {
                items.Add(new TMPTextViewModel() { Text = "test" + i });
            }
            RandomPropertyItems = items;
        }

        #endregion
    }
}

