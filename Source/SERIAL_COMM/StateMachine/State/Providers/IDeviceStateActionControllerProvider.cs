using SERIAL_COMM.StateMachine.State.Actions.Controllers;
using SERIAL_COMM.StateMachine.State.Management;

namespace SERIAL_COMM.StateMachine.State.Providers
{
    interface IDeviceStateActionControllerProvider
    {
        IDeviceStateActionController GetStateActionController(IDeviceStateManager manager);
    }
}
