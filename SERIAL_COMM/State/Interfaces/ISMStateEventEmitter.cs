using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.State.Enums;

namespace SERIAL_COMM.State.Interfaces
{
    //public delegate void OnRequestReceived(LinkRequest request);
    public delegate void OnWorkflowStopped(SMWorkflowStopReason reason);
    public delegate void OnStateChange(SMWorkflowState oldState, SMWorkflowState newState);

    public interface ISMStateEventEmitter
    {
        //event OnRequestReceived RequestReceived;
        event OnWorkflowStopped WorkflowStopped;
        event OnStateChange StateChange;
        //DeviceEventHandler DeviceEventReceived { get; set; }
        ComPortEventHandler ComPortEventReceived { get; set; }
    }
}
