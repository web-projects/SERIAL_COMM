using Microsoft.Extensions.Configuration;

namespace DEVICE_CORE.Config
{
    public interface IDeviceConfigurationProvider
    {
        void InitializeConfiguration();
        IConfiguration GetConfiguration();
        DeviceSection GetAppConfig();
        DeviceSection GetAppConfig(IConfiguration configuration);
    }
}
