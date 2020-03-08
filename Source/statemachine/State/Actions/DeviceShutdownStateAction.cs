using StateMachine.State.Enums;
using StateMachine.State.Interfaces;

namespace StateMachine.State.Actions
{
    internal class DeviceShutdownStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.Shutdown;

        public DeviceShutdownStateAction(IDeviceStateController _) : base(_) { }
    }
}
