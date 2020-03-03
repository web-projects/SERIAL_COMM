using DEVICE_CORE.StateMachine.State.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.Management;

namespace DEVICE_CORE.StateMachine.State.Providers
{
    interface IDeviceStateActionControllerProvider
    {
        IDeviceStateActionController GetStateActionController(IDeviceStateManager manager);
    }
}
