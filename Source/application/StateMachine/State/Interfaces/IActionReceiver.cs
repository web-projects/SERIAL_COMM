using DEVICE_CORE.Helpers;

namespace DEVICE_CORE.StateMachine.State.Interfaces
{
    internal interface IActionReceiver
    {
        //void RequestReceived(LinkRequest request);
        //void DeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation);
        void ComportEventReceived(PortEventType comPortEvent, string portNumber);
    }
}
