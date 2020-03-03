using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;

namespace DEVICE_CORE.StateMachine.State.Actions
{
    internal class DeviceShutdownStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.Shutdown;

        public DeviceShutdownStateAction(IDeviceStateController _) : base(_) { }
    }
}
