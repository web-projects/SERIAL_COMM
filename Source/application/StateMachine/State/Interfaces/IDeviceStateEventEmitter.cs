using DEVICE_CORE.StateMachine.State.Enums;
using Devices.Common;
using XO.Requests;

namespace DEVICE_CORE.StateMachine.State.Interfaces
{
    public delegate void OnRequestReceived(LinkRequest request);
    public delegate void OnWorkflowStopped(DeviceWorkflowStopReason reason);
    public delegate void OnStateChange(DeviceWorkflowState oldState, DeviceWorkflowState newState);

    public interface IDeviceStateEventEmitter
    {
        event OnRequestReceived RequestReceived;
        event OnWorkflowStopped WorkflowStopped;
        event OnStateChange StateChange;
        DeviceEventHandler DeviceEventReceived { get; set; }
        ComPortEventHandler ComPortEventReceived { get; set; }
    }
}
