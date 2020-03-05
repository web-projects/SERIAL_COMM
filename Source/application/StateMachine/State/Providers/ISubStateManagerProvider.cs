using DEVICE_CORE.StateMachine.State.SubWorkflows.Management;
using DEVICE_CORE.StateMachine.State.Interfaces;

namespace DEVICE_CORE.StateMachine.State.Providers
{
    internal interface ISubStateManagerProvider
    {
        IDeviceSubStateManager GetSubStateManager(IDeviceStateController controller);
    }
}
