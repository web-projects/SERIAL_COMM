using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using Devices.Verifone.Connection;
using Devices.Verifone.VIPA;
using Ninject;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using XO.Requests;

namespace Devices.Verifone
{
    [Export(typeof(ICardDevice))]
    [Export("Verifone-M400", typeof(ICardDevice))]
    [Export("Verifone-P200", typeof(ICardDevice))]
    [Export("Verifone-P400", typeof(ICardDevice))]
    [Export("Verifone-UX300", typeof(ICardDevice))]
    internal class VerifoneDevice : IDisposable, ICardDevice
    {
        public string Name => StringValueAttribute.GetStringValue(DeviceType.Verifone);

        //public event PublishEvent PublishEvent;
        public event DeviceEventHandler DeviceEventOccured;

        private SerialConnection serialConnection { get; set; }

        private bool IsConnected { get; set; }

        [Inject]
        internal IVIPADevice vipaDevice { get; set; }

        public DeviceInformation DeviceInformation { get; private set; }

        public string ManufacturerConfigID => DeviceType.Verifone.ToString();

        public int SortOrder { get; set; } = -1;

        public VerifoneDevice()
        {

        }

        public object Clone()
        {
            VerifoneDevice clonedObj = new VerifoneDevice();
            return clonedObj;
        }

        public void Dispose()
        {
            vipaDevice?.Dispose();
        }

        bool ICardDevice.IsConnected(object request)
        {
            return IsConnected;
        }

        public void Probe(DeviceConfig config, DeviceInformation deviceInfo, out bool active)
        {
            DeviceInformation = deviceInfo;
            DeviceInformation.Manufacturer = ManufacturerConfigID;
            DeviceInformation.ComPort = deviceInfo.ComPort;

            serialConnection = new SerialConnection(deviceInfo.ComPort);

            vipaDevice = new VIPADevice();
            active = IsConnected = vipaDevice.Connect(serialConnection);
        }

        public List<DeviceInformation> DiscoverDevices()
        {
            List<DeviceInformation> deviceInformation = new List<DeviceInformation>();
            Connection.DeviceDiscovery deviceDiscovery = new Connection.DeviceDiscovery();
            if (deviceDiscovery.FindVerifoneDevices())
            {
                foreach (var device in deviceDiscovery.deviceInfo)
                {
                    if (string.IsNullOrEmpty(device.ProductID) || string.IsNullOrEmpty(device.SerialNumber))
                        throw new Exception("The connected device's PID or SerialNumber did not match with the expected values!");

                    deviceInformation.Add(new DeviceInformation()
                    {
                        ComPort = device.ComPort,
                        ProductIdentification = device.ProductID,
                        SerialNumber = device.SerialNumber,
                        VendorIdentifier = Connection.DeviceDiscovery.VID
                    });

                    System.Diagnostics.Debug.WriteLine($"device: ON PORT={device.ComPort} - VERIFONE MODEL={deviceInformation[deviceInformation.Count - 1].ProductIdentification}, " +
                        $"SN=[{deviceInformation[deviceInformation.Count - 1].SerialNumber}], PORT={deviceInformation[deviceInformation.Count - 1].ComPort}");
                }
            }

            // validate COMM Port
            if (!deviceDiscovery.deviceInfo.Any() || deviceDiscovery.deviceInfo[0].ComPort == null || !deviceDiscovery.deviceInfo[0].ComPort.Any())
            {
                return null;
            }

            return deviceInformation;
        }

        public void DeviceSetIdle()
        {
            Console.WriteLine($"DEVICE: ON PORT={DeviceInformation.ComPort} - SET-IDLE");
        }

        public bool DeviceRecovery()
        {
            Console.WriteLine($"DEVICE: ON PORT={DeviceInformation.ComPort} - DEVICE-RECOVERY");
            return false;
        }

        public List<LinkRequest> GetDeviceResponse(LinkRequest deviceInfo)
        {
            throw new NotImplementedException();
        }

        // ------------------------------------------------------------------------
        // Methods that are mapped for usage in their respective sub-workflows.
        // ------------------------------------------------------------------------
        #region --- subworkflow mapping
        public LinkRequest GetStatus(LinkRequest request)
        {
            throw new NotImplementedException();
        }

        public LinkRequest AbortCommand(LinkRequest request)
        {
            throw new NotImplementedException();
        }

        public LinkRequest ResetDevice(LinkRequest request)
        {
            throw new NotImplementedException();
        }

        #endregion --- subworkflow mapping

        //internal (DeviceInfoObject deviceInfoObject, int VipaResponse) WriteCommand(ReadCommands command)
        //{
        //    (DeviceInfoObject deviceInfoObject, int VipaResponse) deviceResponse = (null, (int)VipaSW1SW2Codes.Failure);

        //    switch (command)
        //    {
        //        case ReadCommands.DEVICE_ABORT:
        //            {
        //                var response = DeviceCommandAbort();
        //                deviceResponse = (null, response.VipaResponse);
        //                break;
        //            }
        //        case ReadCommands.DEVICE_RESET:
        //            {
        //                deviceResponse = DeviceCommandReset();
        //                break;
        //            }
        //    }

        //    return deviceResponse;
        //}
    }
}
