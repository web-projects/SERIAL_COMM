using SERIAL_COMM.Cancellation;

namespace SERIAL_COMM.Providers
{
    internal interface IDeviceCancellationBrokerProvider
    {
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
