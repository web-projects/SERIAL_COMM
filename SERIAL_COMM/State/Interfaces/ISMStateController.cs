using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.State.Actions;
using SERIAL_COMM.State.Interfaces;
using System.Collections.Generic;

namespace IPA5.DAL.Core.State
{
    //internal interface ISMStateController : ISMStateEventEmitter, IStateControlTrigger<ISMStateAction>
    internal interface ISMStateController : ISMStateEventEmitter
    {
        string PluginPath { get; }
        //ICardDevice TargetDevice { get; }
        //List<ICardDevice> TargetDevices { get; }
        //DeviceSection Configuration { get; }
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
        //IDeviceCancellationBroker GetCancellationBroker();
    }
}
