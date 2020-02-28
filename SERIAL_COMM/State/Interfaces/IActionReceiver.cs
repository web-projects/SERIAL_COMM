using SERIAL_COMM.Helpers;

namespace SERIAL_COMM.State.Interfaces
{
    internal interface IActionReceiver
    {
        //void RequestReceived(LinkRequest request);
        //void DeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation);
        void ComportEventReceived(PortEventType comPortEvent, string portNumber);
    }
}
