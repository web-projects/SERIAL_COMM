using System;
using System.Collections.Generic;
using System.Threading;
using XO.Requests;

namespace Devices.Common.Interfaces
{
    //public delegate void PublishEvent(EventTypeType eventType, EventCodeType eventCode,
    //        List<object> devices, object request, string message);

    public interface ICardDevice : ICloneable
    {
        //event PublishEvent PublishEvent;
        event DeviceEventHandler DeviceEventOccured;

        string Name { get; }

        string ManufacturerConfigID { get; }

        int SortOrder { get; set; }

        DeviceInformation DeviceInformation { get; }

        bool IsConnected(object request);

        List<DeviceInformation> DiscoverDevices();

        void Probe(DeviceConfig config, DeviceInformation deviceInfo, out bool dalActive);

        void DeviceSetIdle();

		bool DeviceRecovery();
		
        void Dispose();

        List<LinkRequest> GetDeviceResponse(LinkRequest deviceInfo);

        // ------------------------------------------------------------------------
        // Methods that are mapped for usage in their respective sub-workflows.
        // ------------------------------------------------------------------------
        LinkRequest GetStatus(LinkRequest request);
        LinkRequest AbortCommand(LinkRequest request);
        LinkRequest ResetDevice(LinkRequest request);
    }
}
