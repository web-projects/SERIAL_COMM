using DEVICE_CORE.StateMachine.Cancellation;

namespace DEVICE_CORE.Providers
{
    internal class DeviceCancellationBrokerProviderImpl : IDeviceCancellationBrokerProvider
    {
        public IDeviceCancellationBroker GetDeviceCancellationBroker() => new DeviceCancellationBrokerImpl();
    }
}
