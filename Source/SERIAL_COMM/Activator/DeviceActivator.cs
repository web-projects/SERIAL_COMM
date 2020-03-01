using Ninject;
using SERIAL_COMM;
using SERIAL_COMM.Modules;
using SERIAL_COMM.Providers;
using System;

namespace SERIAL_COMM
{
    public class DeviceActivator
    {
        [Inject]
        internal IDeviceApplicationProvider DeviceApplicationProvider { get; set; }

        public DeviceActivator()
        {
            using (IKernel kernel = new DeviceKernelResolver().ResolveKernel())
            {
                kernel.Inject(this);
            }
        }

        public IDeviceApplication Start(string pluginPath)
        {
            if (string.IsNullOrWhiteSpace(pluginPath))
            {
                throw new ArgumentNullException(nameof(pluginPath));
            }

            IDeviceApplication application = DeviceApplicationProvider.GetDeviceApplication();
            application.Initialize(pluginPath);
            return application;
        }
    }
}
