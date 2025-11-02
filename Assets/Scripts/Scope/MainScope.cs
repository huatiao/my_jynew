using VContainer;
using VContainer.Unity;
using UnityEngine;
using ProjectBase.Service;
using ProjectBase.Procedure;
using Loxodon.Framework.Views;
using ProjectBase.Model;
using ProjectBase.LoxodonFramework;

namespace ProjectBase.Scope
{
    public class MainScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterServices(builder);
            RegisterModels(builder);
            RegisterStates(builder);

            builder.RegisterBuildCallback(OnBuild);

            builder.RegisterEntryPoint<MainEntryPoint>();
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<IAssetService, AssetService>(Lifetime.Singleton);
            builder.Register<StateMachineService>(Lifetime.Singleton);
            builder.Register<UIService>(Lifetime.Singleton);
            builder.Register<IUIViewLocator, ResourcesUIViewLocator>(Lifetime.Singleton);
        }

        private void RegisterModels(IContainerBuilder builder)
        {
            builder.Register<ConfigModel>(Lifetime.Singleton);
            builder.Register<GameRuntimeModel>(Lifetime.Singleton);
            builder.Register<CharacterModel>(Lifetime.Singleton);
            builder.Register<PlayerModel>(Lifetime.Singleton);
        }

        private void RegisterStates(IContainerBuilder builder)
        {
            builder.Register<InitProcedure>(Lifetime.Transient);
            builder.Register<MainMenuProcedure>(Lifetime.Transient);
        }

        private void OnBuild(IObjectResolver container)
        {
            AddProdureStates(container);

            var uiService = container.Resolve<UIService>();
            uiService.InitService();
        }

        private void AddProdureStates(IObjectResolver container)
        {
            var stateMachine = container.Resolve<StateMachineService>();
            stateMachine.AddState<InitProcedure>();
            stateMachine.AddState<MainMenuProcedure>();
        }
    }
}