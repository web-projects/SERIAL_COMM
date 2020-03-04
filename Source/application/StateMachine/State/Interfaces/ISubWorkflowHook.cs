using DEVICE_CORE.State.SubWorkflows;

namespace DEVICE_CORE.State.Interfaces
{
    internal interface ISubWorkflowHook
    {
        void Hook(IDeviceSubStateController controller);
        void UnHook();
    }
}
