using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SERIAL_COMM.Tests.State.TestStubs
{
    //internal class StubGenericDeviceSubStateManager : IDeviceSubStateManager, IDeviceSubStateController, IStateControllerVisitable<ISubWorkflowHook, IDeviceSubStateController>
    internal class StubGenericDeviceSubStateManager
    {
        //public DeviceSection Configuration => throw new NotImplementedException();

        //public ILoggingServiceClient LoggingClient => throw new NotImplementedException();

        //public IListenerConnector Connector => throw new NotImplementedException();

        //public ICardDevice TargetDevice => throw new NotImplementedException();

        //public List<ICardDevice> TargetDevices => throw new NotImplementedException();

        public bool DidTimeoutOccur => throw new NotImplementedException();

        //public DeviceEvent DeviceEvent => throw new NotImplementedException();

        //public event OnSubWorkflowCompleted SubWorkflowComplete;
        //public event OnSubWorkflowError SubWorkflowError;

        //public virtual void Accept(IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> visitor)
        //{

        //}

        //public Task Complete(IDeviceSubStateAction state) => Task.CompletedTask;

        //public void ComportEventReceived(PortEventType comPortEvent, string portNumber)
        //{

        //}

        //public virtual void DeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation)
        //{

        //}

        public void Dispose()
        {

        }

        //public Task Error(IDeviceSubStateAction state) => Task.CompletedTask;

        //public IDeviceCancellationBroker GetDeviceCancellationBroker()
        //{
        //    return null;
        //}

        //public void LaunchWorkflow(WorkflowOptions launchOptions)
        //{

        //}

        //public void RequestReceived(LinkRequest request)
        //{

        //}

        public void SaveState(object stateObject)
        {

        }
    }
}
