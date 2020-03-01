using Ninject;
using SERIAL_COMM.Modules;

namespace SERIAL_COMM.Providers
{
    internal class DeviceApplicationProvider : IDeviceApplicationProvider
    {
        public IDeviceApplication GetDeviceApplication()
        {
            DeviceApplication application = new DeviceApplication();

            using (IKernel kernel = new DeviceKernelResolver().ResolveKernel())
            {
                kernel.Inject(application);
            }

            return application;
        }
    }
}
