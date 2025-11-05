using UnityEngine;
using System.Collections.Generic;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using VContainer;
using ProjectBase.Model;
using ProjectBase.Define;
using ProjectBase.UI.MainMenu;

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
        [Inject] private PlayerModel _playerModel;


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

        private ObservableList<PropertyItemViewModel> _randomPropertyItems;
        public ObservableList<PropertyItemViewModel> RandomPropertyItems
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
            GenerateRandomProperty();
        }

        public void OnBtnInputNameBack()
        {
            CurrentPage = Page.Menu;
        }
        #endregion

        #region RandomPropertyPage
        private class PropertyItemInfo
        {
            public PropertyID PropertyID;
            public bool IsRandom;
            public int RandValue;

            public PropertyItemInfo(PropertyID propertyID, bool isRandom, int randValue)
            {
                PropertyID = propertyID;
                RandValue = randValue;
                IsRandom = isRandom;
            }
        }

        private List<PropertyItemInfo> _propertyItemInfos;

        public void OnBtnPropertyYes()
        {
            foreach (var item in _propertyItemInfos)
            {
                _playerModel.SetProperty(item.PropertyID, item.RandValue);
            }

            // TODO 切换地图场景
        }

        public void OnBtnPropertyNo()
        {
            GenerateRandomProperty();
        }

        private void GenerateRandomProperty()
        {
            // TODO 梳理完游戏数据逻辑后，重构下面随机属性代码

            if (_propertyItemInfos == null)
            {
                _propertyItemInfos = new List<PropertyItemInfo>();
                var items = new ObservableList<PropertyItemViewModel>();

                // 随机属性（前12项为展示属性）
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.MaxMp_Special, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Attack_Special, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Qinggong, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Defence, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.MaxHp_Special, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Heal, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.UsePoison, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.DePoison, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Quanzhang, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Yujian, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Shuadao, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Anqi, true, 0));

                // 不展示属性（随机）
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.MpType, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.Qimen, true, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.HpInc, true, 0));

                // 不展示属性（非随机）
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.MaxHp, false, 0));
                _propertyItemInfos.Add(new PropertyItemInfo(PropertyID.IQ, false, 0));

                foreach (var item in GameConst.ProItemDic)
                {
                    if (item.Key <= PropertyID.Anqi || item.Key == PropertyID.Qimen)
                    {
                        items.Add(_container.CreateViewModel<PropertyItemViewModel>());
                    }
                }
                RandomPropertyItems = items;
            }

            for (int i = 0; i < _propertyItemInfos.Count; i++)
            {
                var propertyID = _propertyItemInfos[i].PropertyID;

                // 随机属性
                if (_propertyItemInfos[i].IsRandom)
                {
                    int value = GetRandomPropertyValue(propertyID);
                    _propertyItemInfos[i].RandValue = value;
                }
                else
                {
                    if (propertyID == PropertyID.MaxHp)
                    {
                        _propertyItemInfos[i].RandValue = _propertyItemInfos[i].RandValue * 3 + 29;
                    }
                    else if (propertyID == PropertyID.IQ)
                    {
                        int seed = UnityEngine.Random.Range(0, 9);
                        if (seed < 2)
                        {
                            _propertyItemInfos[i].RandValue = UnityEngine.Random.Range(0, 35) + 30;
                        }
                        else if (seed <= 7)
                        {
                            _propertyItemInfos[i].RandValue = UnityEngine.Random.Range(0, 20) + 60;
                        }
                        else
                        {
                            _propertyItemInfos[i].RandValue = UnityEngine.Random.Range(0, 20) + 75;
                        }
                    }
                }
            }

            // 刷新随机属性展示
            for (int i = 0; i < 12; i++)
            {
                var itemInfo = _propertyItemInfos[i];
                var conf = GameConst.ProItemDic[itemInfo.PropertyID];
                
                _randomPropertyItems[i].Text = $"{conf.Name}: {itemInfo.RandValue}";
                _randomPropertyItems[i].ActiveItemBg = itemInfo.RandValue == conf.DefaulMax;
            }
        }

        private int GetRandomPropertyValue(PropertyID propertyID)
        {
            var conf = GameConst.ProItemDic[propertyID];
            return UnityEngine.Random.Range(conf.DefaulMin, conf.DefaulMax + 1);
        }

        #endregion
    }
}

