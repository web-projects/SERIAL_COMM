using SERIAL_COMM.CommandLayer;

namespace SERIAL_COMM.Connection
{
    internal interface ISerialConnection
    {
        bool Connect(bool exposeExceptions = false);
        bool Connected();
        void Disconnect(bool exposeExceptions = false);
        void Dispose();
        void WriteSingleCmd(VIPAResponseHandlers responsehandlers, VIPACommand command);
    }
}