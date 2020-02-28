using SERIAL_COMM.Common;
using SERIAL_COMM.Helpers;

namespace SERIAL_COMM.Connection.Interfaces
{
    //public delegate void DeviceEventHandler(DeviceEvent deviceEvent, DeviceInformation deviceInformation);
    public delegate void ComPortEventHandler(PortEventType comPortEvent, string portNumber);
    //public delegate void QueueEventOccured();
}
