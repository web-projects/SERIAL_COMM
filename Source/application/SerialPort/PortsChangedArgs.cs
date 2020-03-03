using DEVICE_CORE.Helpers;
using System;

namespace DEVICE_CORE.SerialPort
{
    public sealed class PortsChangedArgs : EventArgs
    {
        public string[] SerialPorts { get; }
        public PortEventType EventType { get; }

        public PortsChangedArgs(PortEventType eventType, string[] serialPorts)
            => (EventType, SerialPorts) = (eventType, serialPorts);
    }
}
