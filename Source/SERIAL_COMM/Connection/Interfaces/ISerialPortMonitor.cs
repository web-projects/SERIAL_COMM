using System;

namespace SERIAL_COMM.Connection.Interfaces
{
    public interface ISerialPortMonitor : IDisposable
    {
        event ComPortEventHandler ComportEventOccured;
        void StartMonitoring();
        void StopMonitoring();
    }
}
