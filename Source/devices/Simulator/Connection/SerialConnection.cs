using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace Devices.Simulator.Connection
{
    public class SerialConnection : IDisposable, ISerialConnection
    {
        #region --- attributes ---
        private enum ReadErrorLevel
        {
            None,
            Length,
            Invalid_NAD,
            Invalid_PCB,
            Invalid_CombinedBytes,
            Missing_LRC,
            CombinedBytes_MisMatch
        }

        private Thread readThread;
        private bool readContinue = true;

        private System.IO.Ports.SerialPort serialPort;

        private readonly Object ReadResponsesBytesLock = new object();
        private byte[] ReadResponsesBytes = Array.Empty<byte>();
        private List<byte[]> ReadResponseComponentBytes = new List<byte[]>();
        private ResponseBytesHandlerDelegate ResponseBytesHandler;
        public delegate void ResponseBytesHandlerDelegate(byte[] msg);

        private bool lastCDHolding;
        private bool connected;
        private string commPort;

        #endregion --- attributes ---

        #region --- private methods ---

        private void ReadResponses(byte[] responseBytes)
        {

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
            catch (TimeoutException e)
            {
                Console.WriteLine($"SerialConnection: exception=[{e.Message}]");
            }
        }

        #endregion

        #region --- public methods ---

        public SerialConnection(string port)
        {
            commPort = port;
        }

        public bool Connect(bool exposeExceptions = false)
        {
            try
            {
                // Setup read thread
                readThread = new Thread(ReadResponseBytes);

                // Create a new SerialPort object with default settings.
                serialPort = new System.IO.Ports.SerialPort(commPort);

                // Update the Handshake
                serialPort.Handshake = Handshake.None;

                // Set the read/write timeouts
                serialPort.ReadTimeout = 10000;
                serialPort.WriteTimeout = 10000;

                // open serial port
                serialPort.Open();

                // monitor port changes
                //PortsChanged += OnPortsChanged;
                lastCDHolding = serialPort.CDHolding;

                // discard any buffered bytes
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                ResponseBytesHandler += ReadResponses;

                readThread.Start();

                return connected = true;
            }
            catch (Exception e)
            {
                Console.WriteLine($"SerialConnection: exception=[{e.Message}]");

                if (exposeExceptions)
                {
                    throw;
                }
            }

            return false;
        }

        public void Disconnect(bool exposeExceptions = false)
        {
            if (serialPort?.IsOpen ?? false)
            {
                try
                {
                    readContinue = false;
                    Thread.Sleep(1000);

                    //PortsChanged -= OnPortsChanged;

                    serialPort.Close();

                    readThread.Join(1000);
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
            try
            {
                if (lastCDHolding != serialPort?.CDHolding)
                {
                    connected = false;
                    Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"SerialConnection: exception=[{e.Message}]");
            }

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

        #region --- COMMANDS ---

        public void WriteSingleCmd()
        {
            //WriteBytes(cmdBytes);
        }

        #endregion --- COMMANDS ---
    }
}
