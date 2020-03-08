using System;

namespace StateMachine.State.Management
{
    public interface IDeviceStateManager : IDisposable
    {
        void SetPluginPath(string pluginPath);
        void LaunchWorkflow();
        void StopWorkflow();
    }
}
