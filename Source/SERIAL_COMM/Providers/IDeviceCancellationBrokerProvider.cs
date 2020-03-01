using SERIAL_COMM.StateMachine.Cancellation;

namespace SERIAL_COMM.Providers
{
    internal interface IDeviceCancellationBrokerProvider
    {
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
