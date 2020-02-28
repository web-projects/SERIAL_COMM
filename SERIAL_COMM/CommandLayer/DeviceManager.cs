using Ninject;
using SERIAL_COMM.CommandLayer.Extensions;
using SERIAL_COMM.Connection;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Helpers;
using SERIAL_COMM.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SERIAL_COMM.CommandLayer
{
    public class DeviceManager : IDisposable
    {
        public delegate void ResponseTagsHandlerDelegate(List<TLV.TLV> tags, int responseCode, bool cancelled = false);
        internal ResponseTagsHandlerDelegate responseTagsHandler = null;

        public delegate void ResponseTaglessHandlerDelegate(byte[] data, int responseCode, bool cancelled = false);
        internal ResponseTaglessHandlerDelegate responseTaglessHandler = null;

        public delegate void ResponseCLessHandlerDelegate(List<TLV.TLV> tags, int responseCode, int pcb, bool cancelled = false);
        internal ResponseCLessHandlerDelegate responseCLessHandler = null;

        //public TaskCompletionSource<(DeviceInfoObject deviceInfoObject, int VipaResponse)> deviceIdentifier = null;
        public TaskCompletionSource<int> deviceIdentifier = null;

        int responseTagsHandlerSubscribed;

        private SerialConnection connection { get; }

        [Inject]
        public ISerialPortMonitor SerialPortMonitor { get; set; }

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

        internal void WriteCommand(ReadCommands command, CancellationTokenSource cancellationToken, int timeout)
        {
            switch (command)
            {
                case ReadCommands.DEVICE_RESET:
                {
                    DeviceCommandReset(cancellationToken, timeout);
                    break;
                }
            }
        }

        internal async void DeviceCommandReset(CancellationTokenSource cancellationToken, int timeout)
        {
            try
            {
                deviceIdentifier = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);
                responseTagsHandlerSubscribed++;
                responseTagsHandler += GetDeviceInfoResponseHandler;

                VIPACommand command = new VIPACommand { nad = 0x01, pcb = 0x00, cla = 0xD0, ins = 0xFF, p1 = 0x00, p2 = 0x00 };
                WriteSingleCmd(command);

                await TaskCompletionSourceExtension.WaitAsync(deviceIdentifier, cancellationToken.Token, timeout, true);

                int deviceResponse = deviceIdentifier.Task.Result;

                responseTagsHandler -= GetDeviceInfoResponseHandler;
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
                //deviceIdentifier?.TrySetResult((null, responseCode));
                deviceIdentifier?.TrySetResult(responseCode);
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

            //if (responseCode == (int)VipaSW1SW2Codes.Success)
            if (responseCode == 0x9000)
            {
                if (tags?.Count > 0)
                {
                    /*DeviceInfoObject deviceInfoObject = new DeviceInfoObject
                    {
                        linkDeviceResponse = deviceResponse,
                        LinkDALRequestIPA5Object = cardInfo
                    };
                    deviceIdentifier?.TrySetResult((deviceInfoObject, responseCode));*/
                    deviceIdentifier?.TrySetResult(responseCode);
                }
            }
        }
        #endregion --- response handlers ---
    }
}
