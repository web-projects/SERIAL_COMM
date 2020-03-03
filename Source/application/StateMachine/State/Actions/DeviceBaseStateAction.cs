using DEVICE_CORE.Helpers;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using Devices.Common;
using Devices.Common.Helpers;
using System.Threading.Tasks;

namespace DEVICE_CORE.StateMachine.State.Actions
{
    internal abstract class DeviceBaseStateAction : IDeviceStateAction
    {
        public StateException LastException { get; set; }
        public IDeviceStateController Controller { get; }
        public abstract DeviceWorkflowState WorkflowStateType { get; }
        public DeviceWorkflowStopReason StopReason { get; }

        public object StateObject { get; private set; }

        protected DeviceBaseStateAction(IDeviceStateController controller)
        {
            Controller = controller;
            Controller.RequestReceived += RequestReceived;
            Controller.DeviceEventReceived += DeviceEventReceived;
            Controller.ComPortEventReceived += ComportEventReceived;
        }

        public virtual void Dispose()
        {
            if (Controller != null)
            {
                //Controller.RequestReceived -= RequestReceived;
                Controller.DeviceEventReceived -= DeviceEventReceived;
                Controller.ComPortEventReceived -= ComportEventReceived;
            }
        }

        public virtual bool DoDeviceDiscovery()
        {
            return false;
        }

        public virtual Task DoWork()
        {
            _ = Complete(this);

            return Task.CompletedTask;
        }

        //public virtual void RequestReceived(LinkRequest request)

        public virtual void RequestReceived(object request)
        {

        }

        public virtual void DeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation)
        {
            // TODO: currently the workflow supports a single TargetDevice - we need to enhance the code to support 
            // multiple devices
        }

        public void ComportEventReceived(PortEventType comPortEvent, string portNumber)
        {
            // TODO: currently the workflow supports a single TargetDevice - we need to enhance the code to support 
            // multiple devices
        }

        //public ICardDevice FindTargetDevice(LinkDeviceIdentifier deviceIdentifier)
        //{
        //    if (Controller.TargetDevice != null)
        //    {
        //        return Controller.TargetDevice;
        //    }

        //    if (deviceIdentifier == null)
        //    {
        //        return null;
        //    }

        //    return FindMatchingDevice(deviceIdentifier);
        //}

        //private protected ICardDevice FindMatchingDevice(LinkDeviceIdentifier deviceIdentifier)
        //{
        //    ICardDevice cardDevice = null;

        //    foreach (var device in Controller.TargetDevices)
        //    {
        //        if (device.DeviceInformation != null)
        //        {
        //            if (device.DeviceInformation.Manufacturer.Equals(deviceIdentifier.Manufacturer, StringComparison.CurrentCultureIgnoreCase) &&
        //                device.DeviceInformation.Model.Equals(deviceIdentifier.Model, StringComparison.CurrentCultureIgnoreCase) &&
        //                device.DeviceInformation.SerialNumber.Equals(deviceIdentifier.SerialNumber, StringComparison.CurrentCultureIgnoreCase))
        //            {
        //                cardDevice = device;
        //                break;
        //            }
        //        }
        //    }

        //    return cardDevice;
        //}

        protected Task Complete(IDeviceStateAction state) => _ = Task.Run(() => Controller.Complete(state));

        protected Task Error(IDeviceStateAction state) => _ = Task.Run(() => Controller.Error(state));

        public void SetState(object stateObject) => (StateObject) = (stateObject);
    }
}
