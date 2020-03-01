using Ninject;
using SERIAL_COMM.StateMachine.State.Management;
using System.Threading.Tasks;

namespace SERIAL_COMM
{
    internal class DeviceApplication : IDeviceApplication
    {
        [Inject]
        internal IDeviceStateManager DeviceStateManager { get; set; }

        private string pluginPath;

        public void Initialize(string pluginPath) => (this.pluginPath) = (pluginPath);

        public Task Run()
        {
            DeviceStateManager.SetPluginPath(pluginPath);
            _ = Task.Run(() => DeviceStateManager.LaunchWorkflow());
            return Task.CompletedTask;
        }

        public void Shutdown()
        {
            if (DeviceStateManager != null)
            {
                DeviceStateManager.StopWorkflow();
                DeviceStateManager = null;
            }
        }
    }
}
