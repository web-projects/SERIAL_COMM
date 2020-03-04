using DEVICE_CORE.State.SubWorkflows.Actions.Controllers;
using DEVICE_CORE.State.SubWorkflows.Management;

namespace DEVICE_CORE.State.SubWorkflows.Providers
{
    internal interface IDeviceSubStateActionControllerProvider
    {
        IDeviceSubStateActionController GetStateActionController(IDeviceSubStateManager manager);
    }
}
