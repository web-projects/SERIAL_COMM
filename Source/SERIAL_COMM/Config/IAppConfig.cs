namespace SERIAL_COMM.Config
{
    public interface IAppConfig
    {
        DeviceProviderType DeviceProvider { get; }
        IAppConfig SetDeviceProvider(string deviceProviderType);
    }
}
