using SERIAL_COMM.Config;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.StateMachine.Cancellation;
using SERIAL_COMM.StateMachine.State.Actions;

namespace SERIAL_COMM.StateMachine.State.Interfaces
{
    internal interface IDeviceStateController : IDeviceStateEventEmitter, IStateControlTrigger<IDeviceStateAction>
    {
        string PluginPath { get; }
        //ICardDevice TargetDevice { get; }
        //List<ICardDevice> TargetDevices { get; }
        DeviceSection Configuration { get; }
        //IDevicePluginLoader DevicePluginLoader { get; set; }
        ISerialPortMonitor SerialPortMonitor { get; }
        //PriorityQueue<PriorityQueueDeviceEvents> PriorityQueue { get; set; }
        //ILoggingServiceClient LoggingClient { get; }
        //IListenerConnector Connector { get; }
        //List<ICardDevice> AvailableCardDevices { get; }
        //void SetTargetDevice(ICardDevice targetDevice);
        //void SetTargetDevices(List<ICardDevice> targetDevices);
        //void SetPublishEventHandlerAsTask();
        void SaveState(object stateObject);
        //IControllerVisitorProvider GetCurrentVisitorProvider();
        //ISubStateManagerProvider GetSubStateManagerProvider();
        IDeviceCancellationBroker GetCancellationBroker();
    }
}
