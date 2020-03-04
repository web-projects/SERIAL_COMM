using DEVICE_CORE.Config;
using DEVICE_CORE.State.SubWorkflows.Actions;
using DEVICE_CORE.StateMachine.Cancellation;
using DEVICE_CORE.StateMachine.State;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using System.Collections.Generic;

namespace DEVICE_CORE.State.SubWorkflows
{
    internal interface IDeviceSubStateController : IStateControlTrigger<IDeviceSubStateAction>
    {
        DeviceSection Configuration { get; }
        //ILoggingServiceClient LoggingClient { get; }
        //IListenerConnector Connector { get; }
        ICardDevice TargetDevice { get; }
        List<ICardDevice> TargetDevices { get; }
        bool DidTimeoutOccur { get; }
        public DeviceEvent DeviceEvent { get; }
        void SaveState(object stateObject);
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
