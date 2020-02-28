using SERIAL_COMM.Cancellation;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Providers;

namespace SERIAL_COMM.CommandLayer
{
    internal interface IDeviceManager
    {
        ComPortEventHandler ComPortEventReceived { get; set; }
        ISerialPortMonitor SerialPortMonitor { get; set; }
        bool Connect();
        bool Connected();
        void Dispose();
        void Initialize();
        IDeviceCancellationBroker GetDeviceCancellationBroker();
        IDeviceCancellationBrokerProvider DeviceCancellationBrokerProvider { get; set; }
    }
}