
namespace Devices.Verifone.Interfaces
{
    internal interface IVerifone
    {
        bool Connect();
        bool Connected();
        void Dispose();
    }
}