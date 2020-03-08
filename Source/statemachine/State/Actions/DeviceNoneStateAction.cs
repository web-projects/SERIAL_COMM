using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using System.Threading.Tasks;

namespace StateMachine.State.Actions
{
    internal class DeviceNoneStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.None;

        public DeviceNoneStateAction(IDeviceStateController _) : base(_) { }

        public override Task DoWork()
        {
            _ = Complete(this);

            return Task.CompletedTask;
        }
    }
}
