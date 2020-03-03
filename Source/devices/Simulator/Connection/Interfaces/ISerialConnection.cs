namespace Devices.Simulator.Connection
{
    internal interface ISerialConnection
    {
        bool Connect(bool exposeExceptions = false);
        bool Connected();
        void Disconnect(bool exposeExceptions = false);
        void Dispose();
        void WriteSingleCmd();
    }
}