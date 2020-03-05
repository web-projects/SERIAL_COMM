using DEVICE_CORE.StateMachine.State.Enums;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows.Actions.Controllers
{
    internal interface IDeviceSubStateActionController
    {
        IDeviceSubStateAction GetFinalState();
        IDeviceSubStateAction GetNextAction(IDeviceSubStateAction stateAction);
        IDeviceSubStateAction GetNextAction(DeviceSubWorkflowState state);
    }
}
