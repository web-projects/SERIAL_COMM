using Ninject.Modules;
using SERIAL_COMM.Providers;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Connection.SerialPort;
using SERIAL_COMM.StateMachine.State.Providers;
using SERIAL_COMM.StateMachine.State.Management;

namespace SERIAL_COMM.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDeviceApplicationProvider>().To<DeviceApplicationProvider>();
            //Bind<IDeviceConfigurationProvider>().To<DeviceConfigurationProvider>();
            Bind<IDeviceStateActionControllerProvider>().To<DeviceStateActionControllerProvider>();
            Bind<IDeviceStateManager>().To<DeviceStateManagerImpl>();
            //Bind<ISubStateManagerProvider>().To<SubStateManagerProviderImpl>();
            //Bind<IControllerVisitorProvider>().To<ControllerVisitorProvider>();
            Bind<ISerialPortMonitor>().To<SerialPortMonitor>();
            Bind<IDeviceCancellationBrokerProvider>().To<DeviceCancellationBrokerProviderImpl>();
            Bind<DeviceActivator>().ToSelf();
            Bind<DeviceApplication>().ToSelf();
        }
    }
}
