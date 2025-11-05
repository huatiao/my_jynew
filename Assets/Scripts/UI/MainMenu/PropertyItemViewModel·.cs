using UnityEngine;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using VContainer;
using ProjectBase.Model;

namespace ProjectBase.UI.MainMenu
{
    public class PropertyItemViewModel : DIViewModelBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }

        private bool _activeItemBg;
        public bool ActiveItemBg
        {
            get { return _activeItemBg; }
            set { Set(ref _activeItemBg, value); }
        }
    }
}

