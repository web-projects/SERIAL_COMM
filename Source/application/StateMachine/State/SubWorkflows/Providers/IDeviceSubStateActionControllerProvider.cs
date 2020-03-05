using DEVICE_CORE.StateMachine.State.SubWorkflows.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.SubWorkflows.Management;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows.Providers
{
    internal interface IDeviceSubStateActionControllerProvider
    {
        IDeviceSubStateActionController GetStateActionController(IDeviceSubStateManager manager);
    }
}
