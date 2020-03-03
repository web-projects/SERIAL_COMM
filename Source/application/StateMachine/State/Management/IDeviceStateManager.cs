using System;

namespace DEVICE_CORE.StateMachine.State.Management
{
    public interface IDeviceStateManager : IDisposable
    {
        void SetPluginPath(string pluginPath);
        void LaunchWorkflow();
        void StopWorkflow();
    }
}
