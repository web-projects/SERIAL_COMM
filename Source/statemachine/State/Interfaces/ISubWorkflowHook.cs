using StateMachine.State.SubWorkflows;

namespace StateMachine.State.Interfaces
{
    internal interface ISubWorkflowHook
    {
        void Hook(IDeviceSubStateController controller);
        void UnHook();
    }
}
