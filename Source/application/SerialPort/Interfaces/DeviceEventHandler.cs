using DEVICE_CORE.Helpers;
using Devices.Common;

namespace DEVICE_CORE.SerialPort.Interfaces
{
    public delegate void DeviceEventHandler(DeviceEvent deviceEvent, DeviceInformation deviceInformation);
    public delegate void ComPortEventHandler(PortEventType comPortEvent, string portNumber);
    public delegate void QueueEventOccured();
}
