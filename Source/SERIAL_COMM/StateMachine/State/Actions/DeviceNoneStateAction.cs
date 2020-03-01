using SERIAL_COMM.StateMachine.State.Enums;
using SERIAL_COMM.StateMachine.State.Interfaces;
using System.Threading.Tasks;

namespace SERIAL_COMM.StateMachine.State.Actions
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
