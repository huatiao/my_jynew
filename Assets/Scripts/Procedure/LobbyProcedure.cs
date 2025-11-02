using System;
using UnityEngine;
using UnityHFSM;
using VContainer;
using ProjectBase.Service;
using ProjectBase.UI;
using Cysharp.Threading.Tasks;

namespace ProjectBase.Procedure
{
    public class MainMenuProcedure : StateBase
    {
        [Inject]
        private UIService _uiService;

        public MainMenuProcedure()
            : base(false)
        {

        }

        public override void OnEnter()
        {
            Debug.Log("MainMenuProcedure OnEnter");

            _uiService.OpenUI<MainMenuWindow>();
        }

        public override void OnExit()
        {
            Debug.Log("MainMenuProcedure OnExit");
        }
    }
}