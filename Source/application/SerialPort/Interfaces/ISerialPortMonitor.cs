using System;

namespace DEVICE_CORE.SerialPort.Interfaces
{
    public interface ISerialPortMonitor : IDisposable
    {
        event ComPortEventHandler ComportEventOccured;
        void StartMonitoring();
        void StopMonitoring();
    }
}
