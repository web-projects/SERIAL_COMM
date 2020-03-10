using Config;
using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using StateMachine.Cancellation;
using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;
using StateMachine.State.SubWorkflows.Actions;
using StateMachine.State.SubWorkflows.Management;
using StateMachine.State.Visitors;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XO.Requests;

namespace StateMachine.State.TestStubs.Tests
{
    internal class StubGenericDeviceSubStateManager : IDeviceSubStateManager, IDeviceSubStateController, IStateControllerVisitable<ISubWorkflowHook, IDeviceSubStateController>
    {
        public DeviceSection Configuration => throw new NotImplementedException();

        //public ILoggingServiceClient LoggingClient => throw new NotImplementedException();

        //public IListenerConnector Connector => throw new NotImplementedException();

        public List<ICardDevice> TargetDevices => throw new NotImplementedException();

        public bool DidTimeoutOccur => throw new NotImplementedException();

        public DeviceEvent DeviceEvent => throw new NotImplementedException();

        public event OnSubWorkflowCompleted SubWorkflowComplete;
        public event OnSubWorkflowError SubWorkflowError;

        public virtual void Accept(IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> visitor)
        {

        }

        public Task Complete(IDeviceSubStateAction state) => Task.CompletedTask;

        public void ComportEventReceived(PortEventType comPortEvent, string portNumber)
        {

        }

        public virtual void DeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation)
        {

        }

        public void Dispose()
        {

        }

        public Task Error(IDeviceSubStateAction state) => Task.CompletedTask;

        public IDeviceCancellationBroker GetDeviceCancellationBroker()
        {
            return null;
        }

        public void LaunchWorkflow(WorkflowOptions launchOptions)
        {

        }

        public void RequestReceived(LinkRequest request)
        {

        }

        public void SaveState(object stateObject)
        {

        }
    }
}
