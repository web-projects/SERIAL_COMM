using DEVICE_CORE.StateMachine.State.SubWorkflows.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.SubWorkflows.Management;
using System;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows.Providers
{
    internal class DeviceSubStateActionControllerProvider : IDeviceSubStateActionControllerProvider
    {
        public IDeviceSubStateActionController GetStateActionController(IDeviceSubStateManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            return new DeviceStateActionSubControllerImpl(manager);
        }
    }
}
