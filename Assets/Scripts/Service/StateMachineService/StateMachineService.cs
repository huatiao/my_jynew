using VContainer;
using VContainer.Unity;
using UnityEngine;
using UnityHFSM;

namespace ProjectBase.Service
{
    /// <summary>
    /// 状态机服务（全局）
    /// </summary>
    public class StateMachineService
    {
        [Inject]
        private IObjectResolver _container;
        private StateMachine _fsm;

        public StateMachineService()
        {
            _fsm = new StateMachine();
            _fsm.AddState("None");
            _fsm.Init();
        }

        public void AddState<T>() where T : StateBase
        {
            _fsm.AddState(typeof(T).Name, _container.Resolve<T>());
        }

        public void ChangeState<T>() where T : StateBase
        {
            _fsm.RequestStateChange(typeof(T).Name);
        }
    }
}