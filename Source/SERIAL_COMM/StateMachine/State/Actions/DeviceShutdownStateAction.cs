using SERIAL_COMM.StateMachine.State.Enums;
using SERIAL_COMM.StateMachine.State.Interfaces;

namespace SERIAL_COMM.StateMachine.State.Actions
{
    internal class DeviceShutdownStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.Shutdown;

        public DeviceShutdownStateAction(IDeviceStateController _) : base(_) { }
    }
}
