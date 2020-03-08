using Devices.Common;
using System;

namespace StateMachine.SerialPort.Interfaces
{
    public interface ISerialPortMonitor : IDisposable
    {
        event ComPortEventHandler ComportEventOccured;
        void StartMonitoring();
        void StopMonitoring();
    }
}
