using SERIAL_COMM.Helpers;
using System;

namespace SERIAL_COMM.Connection.SerialPort
{
    public sealed class PortsChangedArgs : EventArgs
    {
        public string[] SerialPorts { get; }
        public PortEventType EventType { get; }

        public PortsChangedArgs(PortEventType eventType, string[] serialPorts)
            => (EventType, SerialPorts) = (eventType, serialPorts);
    }
}
