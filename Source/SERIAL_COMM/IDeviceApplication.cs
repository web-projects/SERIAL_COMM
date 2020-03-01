using System;
using System.Threading.Tasks;

namespace SERIAL_COMM
{
    public interface IDeviceApplication
    {
        void Initialize(string pluginPath);
        Task Run();
        void Shutdown();
    }
}
