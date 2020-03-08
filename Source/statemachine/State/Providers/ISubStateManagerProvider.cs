using StateMachine.State.SubWorkflows.Management;
using StateMachine.State.Interfaces;

namespace StateMachine.State.Providers
{
    internal interface ISubStateManagerProvider
    {
        IDeviceSubStateManager GetSubStateManager(IDeviceStateController controller);
    }
}
