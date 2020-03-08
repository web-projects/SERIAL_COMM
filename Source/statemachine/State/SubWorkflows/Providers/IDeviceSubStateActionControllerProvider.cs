using StateMachine.State.SubWorkflows.Actions.Controllers;
using StateMachine.State.SubWorkflows.Management;

namespace StateMachine.State.SubWorkflows.Providers
{
    internal interface IDeviceSubStateActionControllerProvider
    {
        IDeviceSubStateActionController GetStateActionController(IDeviceSubStateManager manager);
    }
}
