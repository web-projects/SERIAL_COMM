using StateMachine.State.Actions.Controllers;
using StateMachine.State.Management;
using System;

namespace StateMachine.State.Providers
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
