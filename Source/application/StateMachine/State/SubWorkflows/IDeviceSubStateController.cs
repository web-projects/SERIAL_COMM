using DEVICE_CORE.Config;
using DEVICE_CORE.StateMachine.State.SubWorkflows.Actions;
using DEVICE_CORE.StateMachine.Cancellation;
using DEVICE_CORE.StateMachine.State;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using System.Collections.Generic;
using XO.Requests;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows
{
    internal interface IDeviceSubStateController : IStateControlTrigger<IDeviceSubStateAction>
    {
        DeviceSection Configuration { get; }
        //ILoggingServiceClient LoggingClient { get; }
        //IListenerConnector Connector { get; }
        List<ICardDevice> TargetDevices { get; }
        bool DidTimeoutOccur { get; }
        public DeviceEvent DeviceEvent { get; }
        void SaveState(LinkRequest stateObject);
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
