using DEVICE_CORE.StateMachine.State.Enums;

namespace DEVICE_CORE.StateMachine.State.Actions.Controllers
{
    internal interface IDeviceStateActionController
    {
        IDeviceStateAction GetFinalState();
        IDeviceStateAction GetNextAction(IDeviceStateAction stateAction);
        IDeviceStateAction GetNextAction(DeviceWorkflowState state);
        IDeviceStateAction GetSpecificAction(DeviceWorkflowState state);
    }
}
