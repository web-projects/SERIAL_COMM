using Devices.Verifone.VIPA;

namespace Devices.Verifone.Connection
{
    internal interface ISerialConnection
    {
        bool Connect(string port, bool exposeExceptions = false);
        bool Connected();
        void Disconnect(bool exposeExceptions = false);
        void Dispose();
        void WriteSingleCmd(VIPAResponseHandlers responsehandlers, VIPACommand command);
    }
}