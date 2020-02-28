using System;

namespace SERIAL_COMM.State.Management
{
    public interface ISMStateManager : IDisposable
    {
        void SetPluginPath(string pluginPath);
        void LaunchWorkflow();
        void StopWorkflow();
    }
}
