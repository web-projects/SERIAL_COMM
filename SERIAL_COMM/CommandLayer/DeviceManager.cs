using Ninject;
using SERIAL_COMM.Cancellation;
using SERIAL_COMM.CommandLayer.Extensions;
using SERIAL_COMM.CommandLayer.Helpers;
using SERIAL_COMM.CommandLayer.VIPA;
using SERIAL_COMM.Connection;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Helpers;
using SERIAL_COMM.Modules;
using SERIAL_COMM.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SERIAL_COMM.CommandLayer
{
    internal class DeviceManager : IDisposable, IDeviceManager
    {
        #region --- attributes ---
        private enum ResetDeviceCfg
        {
            ReturnSerialNumber = 1 << 0,
            ReturnAfterCardRemoval = 1 << 1,
            LeaveScreenDisplayUnchanged = 1 << 2,
            SlideShowStartsNormalTiming = 1 << 3,
            NoBeepDuringReset = 1 << 4,
            ResetImmediately = 1 << 5,
            ReturnPinpadConfiguration = 1 << 6,
            AddVOSComponentsInformation = 1 << 7
        }

        public TaskCompletionSource<int> responseCodeResult = null;

        public delegate void ResponseTagsHandlerDelegate(List<TLV.TLV> tags, int responseCode, bool cancelled = false);
        internal ResponseTagsHandlerDelegate responseTagsHandler = null;

        public delegate void ResponseTaglessHandlerDelegate(byte[] data, int responseCode, bool cancelled = false);
        internal ResponseTaglessHandlerDelegate responseTaglessHandler = null;

        public delegate void ResponseCLessHandlerDelegate(List<TLV.TLV> tags, int responseCode, int pcb, bool cancelled = false);
        internal ResponseCLessHandlerDelegate responseCLessHandler = null;

        public TaskCompletionSource<(DeviceInfoObject deviceInfoObject, int VipaResponse)> deviceIdentifier = null;

        int responseTagsHandlerSubscribed;

        #endregion --- attributes ---

        [Inject]
        public ISerialPortMonitor SerialPortMonitor { get; set; }

        [Inject]
        public IDeviceCancellationBrokerProvider DeviceCancellationBrokerProvider { get; set; }

        private SerialConnection connection { get; }

        //public DeviceEventHandler DeviceEventReceived { get; set; }

        public ComPortEventHandler ComPortEventReceived { get; set; }

        public DeviceManager(string serialPort)
        {
            connection = new SerialConnection(serialPort);

            using (IKernel kernel = new KernelResolver().ResolveKernel())
            {
                kernel.Inject(this);
            }

            Initialize();
        }

        public void Dispose()
        {
            DestroyComportMonitoring();

            connection?.Dispose();
        }

        public IDeviceCancellationBroker GetCancellationBroker() => DeviceCancellationBrokerProvider.GetDeviceCancellationBroker();

        public IDeviceCancellationBroker GetDeviceCancellationBroker() => this.GetCancellationBroker();

        public bool Connect()
        {
            return connection.Connect();
        }

        public void Initialize()
        {
            //DeviceEventReceived = OnDeviceEventReceived;
            ComPortEventReceived = OnComPortEventReceived;

            SerialPortMonitor.ComportEventOccured += OnComPortEventReceived;
            SerialPortMonitor.StartMonitoring();
        }

        private void OnComPortEventReceived(PortEventType comPortEvent, string portNumber)
        {
            if (comPortEvent == PortEventType.Insertion)
            {
                Console.WriteLine($"Comport Plugged. ComportNumber '{portNumber}'. Detecting a new connection...");
            }
            else if (comPortEvent == PortEventType.Removal)
            {
                Console.WriteLine($"Comport unplugged. ComportNumber '{portNumber}'");
                connection.Dispose();
            }
        }

        public bool Connected()
        {
            return connection?.Connected() ?? false;
        }

        private void DestroyComportMonitoring()
        {
            if (SerialPortMonitor != null)
            {
                SerialPortMonitor.ComportEventOccured -= OnComPortEventReceived;
                SerialPortMonitor.StopMonitoring();
            }
        }

        internal (DeviceInfoObject deviceInfoObject, int VipaResponse) WriteCommand(ReadCommands command)
        {
            (DeviceInfoObject deviceInfoObject, int VipaResponse) deviceResponse = (null, (int)VipaSW1SW2Codes.Failure);

            switch (command)
            {
                case ReadCommands.DEVICE_ABORT:
                {
                    var response = DeviceCommandAbort();
                    deviceResponse = (null, response.VipaResponse);
                    break;
                }
                case ReadCommands.DEVICE_RESET:
                {
                    deviceResponse = DeviceCommandReset();
                    break;
                }
            }

            return deviceResponse;
        }

        internal (int VipaData, int VipaResponse) DeviceCommandAbort()
        {
            (int VipaData, int VipaResponse) deviceResponse = (-1, (int)VipaSW1SW2Codes.Failure);

            responseCodeResult = new TaskCompletionSource<int>();

            try
            {
                deviceIdentifier = new TaskCompletionSource<(DeviceInfoObject deviceInfoObject, int VipaResponse)>(TaskCreationOptions.RunContinuationsAsynchronously);
                responseTagsHandlerSubscribed++;
                responseTagsHandler += ResponseCodeHandler;

                VIPACommand command = new VIPACommand { nad = 0x01, pcb = 0x00, cla = 0xD0, ins = 0xFF, p1 = 0x00, p2 = 0x00 };
                WriteSingleCmd(command);

                deviceResponse = ((int)VipaSW1SW2Codes.Success, responseCodeResult.Task.Result);

                responseTagsHandler -= ResponseCodeHandler;
                responseTagsHandlerSubscribed--;
            }
            catch (TimeoutException e)
            {
                Console.WriteLine("\r\n=========================== RESETDEVICE ERROR ===========================");
                Console.WriteLine($"{DateTime.Now.ToString("yyyyMMdd:HHmmss")}: {e.Message}");
                Console.WriteLine("===============================================================================\r\n");
            }
            catch (OperationCanceledException op)
            {
                Console.WriteLine("{0}: (1) DeviceManager::ResetDevice - EXCEPTION=[{1}]", DateTime.Now.ToString("yyyyMMdd:HHmmss"), op.Message);
            }

            return deviceResponse;
        }

        public (DeviceInfoObject deviceInfoObject, int VipaResponse) DeviceCommandReset()
        {
            (DeviceInfoObject deviceInfoObject, int VipaResponse) deviceResponse = (null, (int)VipaSW1SW2Codes.Failure);

            deviceIdentifier = new TaskCompletionSource<(DeviceInfoObject deviceInfoObject, int VipaResponse)>(TaskCreationOptions.RunContinuationsAsynchronously);

            responseTagsHandlerSubscribed++;
            responseTagsHandler += GetDeviceInfoResponseHandler;

            VIPACommand command = new VIPACommand { nad = 0x01, pcb = 0x00, cla = 0xD0, ins = 0xFF, p1 = 0x00, p2 = 0x00 };
            WriteSingleCmd(command);

            command = new VIPACommand { nad = 0x01, pcb = 0x00, cla = 0xD0, ins = 0x00, p1 = 0x00, p2 = (byte)(ResetDeviceCfg.ReturnSerialNumber | ResetDeviceCfg.ReturnAfterCardRemoval | ResetDeviceCfg.ReturnPinpadConfiguration) };
            WriteSingleCmd(command);   // Device Info [D0, 00]

            deviceResponse = deviceIdentifier.Task.Result;

            responseTagsHandler -= GetDeviceInfoResponseHandler;
            responseTagsHandlerSubscribed--;

            return deviceResponse;
        }

        private void WriteSingleCmd(VIPACommand command)
        {
            connection?.WriteSingleCmd(new VIPAResponseHandlers
            {
                responsetagshandler = responseTagsHandler,
                responsetaglesshandler = responseTaglessHandler,
                responsecontactlesshandler = responseCLessHandler
            }, command);
        }

        #region --- response handlers ---
        public void ResponseCodeHandler(List<TLV.TLV> tags, int responseCode, bool cancelled = false)
        {
            responseCodeResult?.TrySetResult(cancelled ? -1 : responseCode);
        }

        private void GetDeviceInfoResponseHandler(List<TLV.TLV> tags, int responseCode, bool cancelled = false)
        {
            var eeTemplateTag = new byte[] { 0xEE };                // EE Template tag
            var terminalNameTag = new byte[] { 0xDF, 0x0D };        // Terminal Name tag
            var terminalIdTag = new byte[] { 0x9F, 0x1C };          // Terminal ID tag
            var serialNumberTag = new byte[] { 0x9F, 0x1E };        // Serial Number tag
            var tamperStatus = new byte[] { 0xDF, 0x81, 0x01 };     // Tamper Status tag
            var arsStatus = new byte[] { 0xDF, 0x81, 0x02 };        // ARS Status tag

            var efTemplateTag = new byte[] { 0xEF };                // EF Template tag
            var whiteListHash = new byte[] { 0xDF, 0xDB, 0x09 };    // Whitelist tag

            if (cancelled)
            {
                deviceIdentifier?.TrySetResult((null, responseCode));
                return;
            }

            /*var deviceResponse = new LinkDeviceResponse
            {
                // TODO: rework to be values reflecting actual device capabilities
                CardWorkflowControls = new XO.Common.DAL.LinkCardWorkflowControls
                {
                    CardCaptureTimeout = 90,
                    ManualCardTimeout = 5,
                    DebitEnabled = false,
                    EMVEnabled = false,
                    ContactlessEnabled = false,
                    ContactlessEMVEnabled = false,
                    CVVEnabled = false,
                    VerifyAmountEnabled = false,
                    AVSEnabled = false,
                    SignatureEnabled = false
                }
            };

            LinkDALRequestIPA5Object cardInfo = new LinkDALRequestIPA5Object();*/

            foreach (var tag in tags)
            {
                if (tag.Tag.SequenceEqual(eeTemplateTag))
                {
                    foreach (var dataTag in tag.InnerTags)
                    {
                        if (dataTag.Tag.SequenceEqual(terminalNameTag))
                        {
                            //deviceResponse.Model = Encoding.UTF8.GetString(dataTag.Data);
                        }
                        else if (dataTag.Tag.SequenceEqual(serialNumberTag))
                        {
                            //deviceResponse.SerialNumber = Encoding.UTF8.GetString(dataTag.Data);
                            //deviceInformation.SerialNumber = deviceResponse.SerialNumber ?? string.Empty;
                        }
                        else if (dataTag.Tag.SequenceEqual(tamperStatus))
                        {
                            //DF8101 = 00 no tamper detected
                            //DF8101 = 01 tamper detected
                            //cardInfo.TamperStatus = Encoding.UTF8.GetString(dataTag.Data);
                        }
                        else if (dataTag.Tag.SequenceEqual(arsStatus))
                        {
                            //DF8102 = 00 ARS not active
                            //DF8102 = 01 ARS active
                            //cardInfo.ArsStatus = Encoding.UTF8.GetString(dataTag.Data);
                        }
                    }

                    break;
                }
                else if (tag.Tag.SequenceEqual(terminalIdTag))
                {
                    //deviceResponse.TerminalId = Encoding.UTF8.GetString(tag.Data);
                }
                else if (tag.Tag.SequenceEqual(efTemplateTag))
                {
                    foreach (var dataTag in tag.InnerTags)
                    {
                        if (dataTag.Tag.SequenceEqual(whiteListHash))
                        {
                            //cardInfo.WhiteListHash = BitConverter.ToString(dataTag.Data).Replace("-", "");
                        }
                    }
                }
            }

            if (responseCode == (int)VipaSW1SW2Codes.Success)
            {
                if (tags?.Count > 0)
                {
                    DeviceInfoObject deviceInfoObject = new DeviceInfoObject
                    {
                        //linkDeviceResponse = deviceResponse,
                        //LinkDALRequestIPA5Object = cardInfo
                    };
                    deviceIdentifier?.TrySetResult((deviceInfoObject, responseCode));
                }
                else
                {
                    deviceIdentifier?.TrySetResult((null, responseCode));
                }
            }
        }
        #endregion --- response handlers ---
    }
}
