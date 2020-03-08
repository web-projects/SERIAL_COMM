using StateMachine.State.SubWorkflows.Actions.Controllers;
using StateMachine.State.SubWorkflows.Management;
using System;

namespace StateMachine.State.SubWorkflows.Providers
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
