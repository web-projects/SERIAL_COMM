using System;
using System.Threading.Tasks;

namespace DEVICE_CORE
{
    public interface IDeviceApplication
    {
        void Initialize(string pluginPath);
        Task Run();
        void Shutdown();
    }
}
