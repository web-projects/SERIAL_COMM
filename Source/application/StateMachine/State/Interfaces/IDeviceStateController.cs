using DEVICE_CORE.Config;
using DEVICE_CORE.SerialPort.Interfaces;
using DEVICE_CORE.State.Interfaces;
using DEVICE_CORE.State.Providers;
using DEVICE_CORE.StateMachine.Cancellation;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_SDK.Sdk;
using Devices.Common.Interfaces;
using System.Collections.Generic;

namespace DEVICE_CORE.StateMachine.State.Interfaces
{
    internal interface IDeviceStateController : IDeviceStateEventEmitter, IStateControlTrigger<IDeviceStateAction>
    {
        string PluginPath { get; }
        DeviceSection Configuration { get; }
        IDevicePluginLoader DevicePluginLoader { get; set; }
        //ICardDevice TargetDevice { get; }
        List<ICardDevice> TargetDevices { get; }
        ISerialPortMonitor SerialPortMonitor { get; }
        //PriorityQueue<PriorityQueueDeviceEvents> PriorityQueue { get; set; }
        //ILoggingServiceClient LoggingClient { get; }
        //IListenerConnector Connector { get; }
        List<ICardDevice> AvailableCardDevices { get; }
        void SetTargetDevices(List<ICardDevice> targetDevices);
        void SetPublishEventHandlerAsTask();
        void SaveState(object stateObject);
        IControllerVisitorProvider GetCurrentVisitorProvider();
        ISubStateManagerProvider GetSubStateManagerProvider();
        IDeviceCancellationBroker GetCancellationBroker();
    }
}
