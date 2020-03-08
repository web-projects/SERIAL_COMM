using StateMachine.State.Enums;

namespace StateMachine.State.Actions.Controllers
{
    internal interface IDeviceStateActionController
    {
        IDeviceStateAction GetFinalState();
        IDeviceStateAction GetNextAction(IDeviceStateAction stateAction);
        IDeviceStateAction GetNextAction(DeviceWorkflowState state);
        IDeviceStateAction GetSpecificAction(DeviceWorkflowState state);
    }
}
