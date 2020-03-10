using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;
using XO.Requests;

namespace StateMachine.State.SubWorkflows.Actions
{
    internal interface IDeviceSubStateAction : IActionReceiver, IDisposable
    {
        bool WorkflowCutoff { get; }
        StateException LastException { get; }
        IDeviceSubStateController Controller { get; }
        DeviceSubWorkflowState WorkflowStateType { get; }
        CancellationToken CancellationToken { get; }
        SubStateActionLaunchRules LaunchRules { get; }
        void SetState(object stateObject);
        void SetCancellationToken(CancellationToken cancellationToken);
        Task DoWork();
    }
}
