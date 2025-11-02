using UnityEngine;
using Loxodon.Framework.Interactivity;
using Loxodon.Framework.Observables;
using VContainer;
using ProjectBase.Model;

namespace ProjectBase.UI
{
    public class TMPTextViewModel : DIViewModelBase
    {
        private string _text;
        public string Text
        {
            get { return _text; }
            set { Set(ref _text, value); }
        }
    }
}

