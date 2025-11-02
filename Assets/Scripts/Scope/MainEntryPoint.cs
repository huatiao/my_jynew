using VContainer;
using VContainer.Unity;
using UnityEngine;
using ProjectBase.Procedure;
using ProjectBase.Service;
using Cysharp.Threading.Tasks;

namespace ProjectBase.Scope
{
    public class MainEntryPoint : IStartable
    {
        [Inject]
        private StateMachineService _stateMachineService;

        public void Start()
        {
            Debug.Log("MainEntryPoint Start");

            _stateMachineService.ChangeState<InitProcedure>();
        }
    }
}