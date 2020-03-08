using Config;
using DEVICE_CORE.Providers;
using Ninject.Modules;
using StateMachine.Providers;
using StateMachine.SerialPort;
using StateMachine.SerialPort.Interfaces;
using StateMachine.State.Management;
using StateMachine.State.Providers;

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
            Bind<ISubStateManagerProvider>().To<SubStateManagerProviderImpl>();
            Bind<IControllerVisitorProvider>().To<ControllerVisitorProvider>();
            Bind<ISerialPortMonitor>().To<SerialPortMonitor>();
            Bind<IDeviceCancellationBrokerProvider>().To<DeviceCancellationBrokerProviderImpl>();
            Bind<DeviceActivator>().ToSelf();
            Bind<DeviceApplication>().ToSelf();
        }
    }
}
