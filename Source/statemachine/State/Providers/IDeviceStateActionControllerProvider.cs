using StateMachine.State.Actions.Controllers;
using StateMachine.State.Management;

namespace StateMachine.State.Providers
{
    interface IDeviceStateActionControllerProvider
    {
        IDeviceStateActionController GetStateActionController(IDeviceStateManager manager);
    }
}
