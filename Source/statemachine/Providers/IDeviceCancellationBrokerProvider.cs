using StateMachine.Cancellation;

namespace StateMachine.Providers
{
    internal interface IDeviceCancellationBrokerProvider
    {
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
