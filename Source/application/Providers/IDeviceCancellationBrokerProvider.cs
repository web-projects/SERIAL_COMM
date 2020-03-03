using DEVICE_CORE.StateMachine.Cancellation;

namespace DEVICE_CORE.Providers
{
    internal interface IDeviceCancellationBrokerProvider
    {
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
