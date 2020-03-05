using DEVICE_CORE.StateMachine.State.SubWorkflows;

namespace DEVICE_CORE.StateMachine.State.Interfaces
{
    internal interface ISubWorkflowHook
    {
        void Hook(IDeviceSubStateController controller);
        void UnHook();
    }
}
