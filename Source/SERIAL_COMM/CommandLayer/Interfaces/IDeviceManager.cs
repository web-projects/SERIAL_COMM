using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Providers;
using SERIAL_COMM.StateMachine.Cancellation;

namespace SERIAL_COMM.CommandLayer.Interfaces
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