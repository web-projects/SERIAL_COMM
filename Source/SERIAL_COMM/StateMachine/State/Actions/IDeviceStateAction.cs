using SERIAL_COMM.StateMachine.State.Enums;
using SERIAL_COMM.StateMachine.State.Interfaces;
using System;
using System.Threading.Tasks;

namespace SERIAL_COMM.StateMachine.State.Actions
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
