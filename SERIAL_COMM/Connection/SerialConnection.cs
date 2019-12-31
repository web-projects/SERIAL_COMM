using SERIAL_COMM.Helpers;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Threading;

namespace SERIAL_COMM.Connection
{
    public class SerialConnection : IDisposable
    {
        #region --- attributes ---
        private Thread readThread;
        private bool readContinue = true;

        private System.IO.Ports.SerialPort serialPort;

        private readonly Object ReadResponsesBytesLock = new object();
        private byte[] ReadResponsesBytes = Array.Empty<byte>();
        private List<byte[]> ReadResponseComponentBytes = new List<byte[]>();
        private ResponseBytesHandlerDelegate ResponseBytesHandler;
        public delegate void ResponseBytesHandlerDelegate(byte[] msg);

        private bool connected;
        private string COMMPort;
        private static string[] SerialPorts;
        private static ManagementEventWatcher arrival;
        private static ManagementEventWatcher removal;

        public static event EventHandler<PortsChangedArgs> PortsChanged;

        #endregion

        #region --- private methods ---

        private void ReadResponses(byte[] responseBytes)
        {
            var validNADValues = new List<byte> { 0x01, 0x02, 0x11 };
            var validPCBValues = new List<byte> { 0x00, 0x01, 0x02, 0x03, 0x40, 0x41, 0x42, 0x43 };
            var nestedTagTags = new List<byte[]> { new byte[] { 0xEE }, new byte[] { 0xEF }, new byte[] { 0xF0 }, new byte[] { 0xE0 }, new byte[] { 0xE4 }, new byte[] { 0xE7 }, new byte[] { 0xFF, 0x7C }, new byte[] { 0xFF, 0x7F } };
            var addedResponseComponent = false;

            lock (ReadResponsesBytesLock)
            {
            }
        }

        [System.Diagnostics.DebuggerNonUserCode]
        private void ReadResponseBytes()
        {
            while (readContinue)
            {
                try
                {
                    byte[] bytes = new byte[256];
                    var readLength = serialPort.Read(bytes, 0, bytes.Length);
                    if (readLength > 0)
                    {
                        byte[] readBytes = new byte[readLength];
                        Array.Copy(bytes, 0, readBytes, 0, readLength);

                        ResponseBytesHandler(readBytes);
#if DEBUG
                        Console.WriteLine($"DEVICE-READ: {BitConverter.ToString(readBytes)}");
                        System.Diagnostics.Debug.WriteLine($"READ: {BitConverter.ToString(readBytes)}");
#endif
                    }
                }
                catch (TimeoutException)
                {
                }
                catch (Exception)
                {
                }
            }
        }

        private void WriteBytes(byte[] msg)
        {
            try
            {
                serialPort.Write(msg, 0, msg.Length);
            }
            catch (TimeoutException)
            {
            }
        }

        #endregion

        #region --- public methods ---

        public SerialConnection(string port)
        {
            COMMPort = port;
            SerialPorts = GetAvailableSerialPorts();
            MonitorDeviceChanges();
        }

        public bool Connect(bool exposeExceptions = false)
        {
            try
            {
                // Setup read thread
                readThread = new Thread(ReadResponseBytes);

                // Create a new SerialPort object with default settings.
                serialPort = new System.IO.Ports.SerialPort(COMMPort);

                // Update the Handshake
                serialPort.Handshake = System.IO.Ports.Handshake.None;

                // Set the read/write timeouts
                serialPort.ReadTimeout = 10000;
                serialPort.WriteTimeout = 10000;

                // open serial port
                serialPort.Open();

                // monitor port changes
                PortsChanged += OnPortsChanged;

                // discard any buffered bytes
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                ResponseBytesHandler += ReadResponses;

                readThread.Start();

                return connected = true;
            }
            catch (Exception)
            {
                if (exposeExceptions)
                {
                    throw;
                }
            }

            return false;
        }

        private void OnPortsChanged(object sender, PortsChangedArgs e)
        {
            switch (e.EventType)
            {
                case EventType.Removal:
                {
                    if (e.SerialPorts.Contains(COMMPort) == false)
                    {
                        readContinue = false;
                        connected = false;
                    }

                    break;
                }

                case EventType.Insertion:
                {
                    break;
                }
            }
        }

        public void Disconnect(bool exposeExceptions = false)
        {
            if (serialPort?.IsOpen ?? false)
            {
                try
                {
                    readContinue = false;
                    Thread.Sleep(1000);

                    PortsChanged -= OnPortsChanged;

                    serialPort.Close();

                    readThread.Join();
                }
                catch (Exception)
                {
                    if (exposeExceptions)
                    {
                        throw;
                    }
                }
            }
        }

        public bool Connected()
        {
            return connected;
        }

        public void Dispose()
        {
            readContinue = false;
            Thread.Sleep(100);

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            Disconnect();

            if (disposing)
            {
                serialPort?.Dispose();
            }
        }

        #endregion

        #region --- EVENTS ---
        private static void MonitorDeviceChanges()
        {
            try
            {
                var deviceArrivalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
                var deviceRemovalQuery = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

                arrival = new ManagementEventWatcher(deviceArrivalQuery);
                removal = new ManagementEventWatcher(deviceRemovalQuery);

                arrival.EventArrived += (o, args) => RaisePortsChangedIfNecessary(EventType.Insertion);
                removal.EventArrived += (sender, eventArgs) => RaisePortsChangedIfNecessary(EventType.Removal);

                // Start listening for events
                arrival.Start();
                removal.Start();
            }
            catch (ManagementException e)
            {
                Console.WriteLine($"serial: COMM exception={e.Message}");
            }
        }

        private static void RaisePortsChangedIfNecessary(EventType eventType)
        {
            lock (SerialPorts)
            {
                var availableSerialPorts = GetAvailableSerialPorts();
                IEnumerable<string> uniqueItems = availableSerialPorts.Distinct<string>();

                if (eventType == EventType.Insertion)
                {
                    if (!SerialPorts.SequenceEqual(uniqueItems))
                    {
                        var added = uniqueItems.Except(SerialPorts).ToArray();
                        SerialPorts = added;
                        PortsChanged.Raise(null, new PortsChangedArgs(eventType, added));
                    }
                }
                else if (eventType == EventType.Removal)
                {
                    if (SerialPorts.SequenceEqual(uniqueItems))
                    {
                        var removed = SerialPorts.Except(uniqueItems).ToArray();
                        SerialPorts = removed;
                        PortsChanged.Raise(null, new PortsChangedArgs(eventType, removed));
                    }
                }
            }
        }

        public static string[] GetAvailableSerialPorts()
        {
            return SerialPort.GetPortNames();
        }

        #endregion
    }
}
