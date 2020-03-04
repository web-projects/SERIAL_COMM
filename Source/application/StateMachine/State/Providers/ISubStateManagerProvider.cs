using DEVICE_CORE.State.SubWorkflows.Management;
using DEVICE_CORE.StateMachine.State.Interfaces;

namespace DEVICE_CORE.State.Providers
{
    internal interface ISubStateManagerProvider
    {
        IDeviceSubStateManager GetSubStateManager(IDeviceStateController controller);
    }
}
