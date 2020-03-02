using Microsoft.Extensions.Configuration;

namespace SERIAL_COMM.Config
{
    public interface IDeviceConfigurationProvider
    {
        void InitializeConfiguration();
        IConfiguration GetConfiguration();
        DeviceSection GetAppConfig();
        DeviceSection GetAppConfig(IConfiguration configuration);
    }
}
