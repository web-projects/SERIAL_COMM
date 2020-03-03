using DEVICE_CORE.Config;
using DEVICE_CORE.Providers;
using DEVICE_CORE.SerialPort;
using DEVICE_CORE.SerialPort.Interfaces;
using DEVICE_CORE.StateMachine.State.Management;
using DEVICE_CORE.StateMachine.State.Providers;
using Ninject.Modules;

namespace DEVICE_CORE.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDeviceApplicationProvider>().To<DeviceApplicationProvider>();
            Bind<IDeviceConfigurationProvider>().To<DeviceConfigurationProvider>();
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
