using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using System.Threading.Tasks;

namespace DEVICE_CORE.StateMachine.State.Actions
{
    internal interface IDeviceStateAction : IActionReceiver, IDisposable
    {
        StateException LastException { get; }
        IDeviceStateController Controller { get; }
        DeviceWorkflowState WorkflowStateType { get; }
        void SetState(object stateObject);
        DeviceWorkflowStopReason StopReason { get; }
        bool DoDeviceDiscovery();
        Task DoWork();
    }
}
