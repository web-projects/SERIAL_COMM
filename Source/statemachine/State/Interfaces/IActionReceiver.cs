using Devices.Common;
using Devices.Common.Helpers;

namespace StateMachine.State.Interfaces
{
    internal interface IActionReceiver
    {
        void RequestReceived(XO.Requests.LinkRequest request);
        void DeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation);
        void ComportEventReceived(PortEventType comPortEvent, string portNumber);
    }
}
