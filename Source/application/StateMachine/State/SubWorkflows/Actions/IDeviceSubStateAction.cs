using DEVICE_CORE.StateMachine.State;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DEVICE_CORE.State.SubWorkflows.Actions
{
    internal interface IDeviceSubStateAction : IActionReceiver, IDisposable
    {
        bool WorkflowCutoff { get; }
        StateException LastException { get; }
        IDeviceSubStateController Controller { get; }
        DeviceSubWorkflowState WorkflowStateType { get; }
        CancellationToken CancellationToken { get; }
        SubStateActionLaunchRules LaunchRules { get; }
        void SetState(LinkRequest stateObject);
        void SetCancellationToken(CancellationToken cancellationToken);
        Task DoWork();
    }
}
