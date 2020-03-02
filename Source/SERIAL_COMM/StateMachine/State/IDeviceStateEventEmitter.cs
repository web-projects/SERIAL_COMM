using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.StateMachine.State.Enums;

namespace SERIAL_COMM.State
{
    //public delegate void OnRequestReceived(LinkRequest request);
    public delegate void OnRequestReceived(object request);
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
