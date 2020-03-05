using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.SubWorkflows.Management;

namespace DEVICE_CORE.StateMachine.State.Providers
{
    internal class SubStateManagerProviderImpl : ISubStateManagerProvider
    {
        public IDeviceSubStateManager GetSubStateManager(IDeviceStateController controller)
            => new GenericSubStateManagerImpl(controller);
    }
}
