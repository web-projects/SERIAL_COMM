using DEVICE_CORE.Providers;
using DEVICE_CORE.StateMachine.Cancellation;

namespace IPA5.DAL.Core.State.Providers
{
    internal class DeviceCancellationBrokerProviderImpl : IDeviceCancellationBrokerProvider
    {
        public IDeviceCancellationBroker GetDeviceCancellationBroker() => new DeviceCancellationBrokerImpl();
    }
}
