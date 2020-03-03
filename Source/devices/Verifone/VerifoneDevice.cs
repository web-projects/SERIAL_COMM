using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using Devices.Verifone.Connection;
using Devices.Verifone.Interfaces;
using Devices.Verifone.VIPA;
using Ninject;
using System;
using System.Composition;
using System.Collections.Generic;
using System.Linq;

namespace Devices.Verifone
{
    [Export(typeof(ICardDevice))]
    [Export("Verifone-M400", typeof(ICardDevice))]
    [Export("Verifone-P200", typeof(ICardDevice))]
    [Export("Verifone-P400", typeof(ICardDevice))]
    [Export("Verifone-UX300", typeof(ICardDevice))]
    internal class VerifoneDevice : IDisposable, ICardDevice, IVerifone
    {
        public string Name => StringValueAttribute.GetStringValue(DeviceType.Verifone);

        //public event PublishEvent PublishEvent;
        public event DeviceEventHandler DeviceEventOccured;

        private SerialConnection serialConnection { get; set;  }

        private bool IsConnected { get; set; }

        [Inject]
        internal IVIPADevice vipaDevice { get; } = new VIPADevice();

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

        public bool Connect()
        {
            IsConnected = vipaDevice.Connect(serialConnection);
            return IsConnected;
        }

        public bool Connected()
        {
            return IsConnected;
        }

        bool ICardDevice.IsConnected(object request)
        {
            return IsConnected;
        }

        public void Probe(DeviceConfig config, DeviceInformation deviceInfo, out bool active)
        {
            DeviceInformation.ComPort = config.SerialConfig.CommPortName;
            serialConnection = new SerialConnection(config.SerialConfig.CommPortName);
            active = IsConnected = serialConnection.Connect();
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

                    System.Diagnostics.Debug.WriteLine($"device: VERIFONE MODEL={deviceInformation[deviceInformation.Count - 1].ProductIdentification}, " +
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
            throw new NotImplementedException();
        }

        public bool DeviceRecovery()
        {
            throw new NotImplementedException();
        }

        public List<object> GetDeviceResponse(object deviceInfo)
        {
            throw new NotImplementedException();
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
