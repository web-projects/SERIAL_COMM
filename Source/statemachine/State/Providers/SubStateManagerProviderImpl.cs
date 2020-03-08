using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows.Management;

namespace StateMachine.State.Providers
{
    internal class SubStateManagerProviderImpl : ISubStateManagerProvider
    {
        public IDeviceSubStateManager GetSubStateManager(IDeviceStateController controller)
            => new GenericSubStateManagerImpl(controller);
    }
}
