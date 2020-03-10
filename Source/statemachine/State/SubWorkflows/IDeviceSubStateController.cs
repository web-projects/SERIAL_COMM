using Config;
using StateMachine.State.SubWorkflows.Actions;
using StateMachine.Cancellation;
using StateMachine.State;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using System.Collections.Generic;
using XO.Requests;

namespace StateMachine.State.SubWorkflows
{
    internal interface IDeviceSubStateController : IStateControlTrigger<IDeviceSubStateAction>
    {
        DeviceSection Configuration { get; }
        //ILoggingServiceClient LoggingClient { get; }
        //IListenerConnector Connector { get; }
        List<ICardDevice> TargetDevices { get; }
        bool DidTimeoutOccur { get; }
        public DeviceEvent DeviceEvent { get; }
        void SaveState(object stateObject);
        IDeviceCancellationBroker GetDeviceCancellationBroker();
    }
}
