using Devices.Verifone.Helpers;
using Devices.Verifone.TLV;
using Devices.Verifone.Connection;
using System.Collections.Generic;

namespace Devices.Verifone.VIPA
{
    public interface IVIPADevice
    {
        bool Connect(SerialConnection connection);
        void Dispose();
        (DeviceInfoObject deviceInfoObject, int VipaResponse) DeviceCommandReset();
        void ResponseCodeHandler(List<TLV.TLV> tags, int responseCode, bool cancelled = false);
    }
}