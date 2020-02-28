using Ninject.Modules;
using SERIAL_COMM.Providers;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Connection.SerialPort;

namespace SERIAL_COMM.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISerialPortMonitor>().To<SerialPortMonitor>();
            Bind<IDeviceCancellationBrokerProvider>().To<DeviceCancellationBrokerProviderImpl>();
        }
    }
}
