using SERIAL_COMM.StateMachine.State.Actions.Controllers;
using SERIAL_COMM.StateMachine.State.Management;
using System;

namespace SERIAL_COMM.StateMachine.State.Providers
{
    class DeviceStateActionControllerProvider : IDeviceStateActionControllerProvider
    {
        public IDeviceStateActionController GetStateActionController(IDeviceStateManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            return new DeviceStateActionControllerImpl(manager);
        }
    }
}
