using System;

namespace DEVICE_CORE.State.SubWorkflows.Providers
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
