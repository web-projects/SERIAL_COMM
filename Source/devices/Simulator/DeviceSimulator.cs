using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using Devices.Simulator.Connection;
using Ninject;

namespace Devices.Simulator
{
    [Export(typeof(ICardDevice))]
    [Export("Simulator Device", typeof(ICardDevice))]
    internal class DeviceSimulator : ICardDevice, IComparable
    {
        public string Name => StringValueAttribute.GetStringValue(DeviceType.Simulator);

        public int SortOrder { get; set; } = -1;

        [Inject]
        private ISerialConnection serialConnection { get; set;  }

        private bool IsConnected { get; set; }

        //public event PublishEvent PublishEvent;
        public event DeviceEventHandler DeviceEventOccured;

        public List<object> GetDeviceResponse(object deviceInfo)
        {
            return null;
        }

        public int CompareTo(object obj)
        {
            var device = obj as ICardDevice;

            if (SortOrder > device.SortOrder)
                return 1;

            if (SortOrder < device.SortOrder)
                return -1;

            return 0;
        }

        public string ManufacturerConfigID => DeviceType.Simulator.ToString();

        public DeviceInformation DeviceInformation { get; private set; }

        public DeviceSimulator()
        {

        }

        public object Clone()
        {
            DeviceSimulator clonedObj = new DeviceSimulator();
            return clonedObj;
        }

        public void Dispose()
        {

        }

        bool ICardDevice.IsConnected(object request)
        {
            return IsConnected;
        }

        public List<DeviceInformation> DiscoverDevices()
        {
            List<DeviceInformation> deviceInformation = new List<DeviceInformation>();
            deviceInformation.Add(new DeviceInformation()
            {
                ComPort = "COM3",
                Manufacturer = ManufacturerConfigID,
                Model = "SimCity",
                SerialNumber = "CEEEDEADBEEF",
                ProductIdentification = "SIMULATOR",
                VendorIdentifier = "BADDCACA"
                
            });

            return deviceInformation;
        }

        public void Probe(DeviceConfig config, DeviceInformation deviceInfo, out bool active)
        {
            DeviceInformation = new DeviceInformation()
            {
                ComPort = config.SerialConfig.CommPortName,
                Manufacturer = ManufacturerConfigID,
                Model = "SimCity",
                SerialNumber = "CEEEDEADBEEF",
                ProductIdentification = "SIMULATOR",
                VendorIdentifier = "BADDCACA"
            };
            deviceInfo = DeviceInformation;
            serialConnection = new SerialConnection(config.SerialConfig.CommPortName);
            active = IsConnected = serialConnection.Connect();
        }

        public void DeviceSetIdle()
        {
        }

        public bool DeviceRecovery()
        {
            return true;
        }

        // ------------------------------------------------------------------------
        // Methods that are mapped for usage in their respective sub-workflows.
        // ------------------------------------------------------------------------
        #region --- subworkflow mapping
        public object GetStatus(object request)
        {
            throw new NotImplementedException();
        }

        public object AbortCommand(object request)
        {
            throw new NotImplementedException();
        }

        public object ResetDevice(object request)
        {
            throw new NotImplementedException();
        }

        #endregion --- subworkflow mapping
    }
}
