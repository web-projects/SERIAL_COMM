using System;
using System.Collections.Generic;
using System.Threading;

namespace Devices.Common.Interfaces
{
    //public delegate void PublishEvent(EventTypeType eventType, EventCodeType eventCode,
    //        List<object> devices, object request, string message);

    public interface ICardDevice : ICloneable
    {
        //event PublishEvent PublishEvent;
        //event DeviceEventHandler DeviceEventOccured;

        string Name { get; }

        string ManufacturerConfigID { get; }

        int SortOrder { get; set; }

        DeviceInformation DeviceInformation { get; }

        bool IsConnected(object request);

        List<DeviceInformation> DiscoverDevices();

        List<object> Probe(DeviceConfig config, DeviceInformation deviceInfo, out bool dalActive);

        void DeviceSetIdle();

		bool DeviceRecovery();
		
        void Dispose();

        List<object> GetDeviceResponse(object deviceInfo);

        // ------------------------------------------------------------------------
        // Methods that are mapped for usage in their respective sub-workflows.
        // ------------------------------------------------------------------------
        object GetStatus(object request);
        object AbortCommand(object request);
        object ResetDevice(object request);
    }
}
