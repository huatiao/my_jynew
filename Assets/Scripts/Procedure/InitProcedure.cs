using System;
using UnityEngine;
using UnityHFSM;
using ProjectBase.Service;
using VContainer;
using ProjectBase.Model;
using Loxodon.Framework.Contexts;
using Loxodon.Framework.Binding;

namespace ProjectBase.Procedure
{
    public class InitProcedure : StateBase
    {
        [Inject] private StateMachineService _stateMachineService;
        [Inject] private ConfigModel _confModel;
        [Inject] private CharacterModel _characterModel;

        public InitProcedure()
            : base(false)
        {
        }

        public override void OnEnter()
        {
            Debug.Log("InitProcedure OnEnter");

            //初始化游戏
            try
            {
                InitGame();
            }
            catch (Exception e)
            {
                Debug.LogError($"InitGame Error: {e.Message}");
                return;
            }

            OnInit();
        }

        public override void OnExit()
        {
            Debug.Log("InitProcedure OnExit");

            //TODO: 清理游戏
        }

        private void InitGame()
        {
            // 初始化MVVM框架数据绑定服务
            ApplicationContext context = Context.GetApplicationContext();
            BindingServiceBundle bindingService = new BindingServiceBundle(context.GetContainer());
            bindingService.Start();

            // 加载配置资源
            if (!_confModel.Init())
            {
                throw new Exception("Config Init Failed");
            }

            // 解析配置资源
            _characterModel.Init();

        }

        private void OnInit()
        {
            // 切换到下一个状态
            _stateMachineService.ChangeState<MainMenuProcedure>();
        }
    }
}