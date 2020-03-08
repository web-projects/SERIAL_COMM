using StateMachine.State.Enums;

namespace StateMachine.State.SubWorkflows.Actions.Controllers
{
    internal interface IDeviceSubStateActionController
    {
        IDeviceSubStateAction GetFinalState();
        IDeviceSubStateAction GetNextAction(IDeviceSubStateAction stateAction);
        IDeviceSubStateAction GetNextAction(DeviceSubWorkflowState state);
    }
}
