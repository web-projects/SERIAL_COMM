using DEVICE_CORE.StateMachine.State.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.Management;
using System;

namespace DEVICE_CORE.StateMachine.State.Providers
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
