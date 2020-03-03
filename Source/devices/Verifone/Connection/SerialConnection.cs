using Devices.Verifone.VIPA;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace Devices.Verifone.Connection
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

        internal VIPADevice.ResponseTagsHandlerDelegate ResponseTagsHandler = null;
        internal VIPADevice.ResponseTaglessHandlerDelegate ResponseTaglessHandler = null;
        internal VIPADevice.ResponseCLessHandlerDelegate ResponseContactlessHandler = null;

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
                // Add current bytes to available bytes
                var combinedResponseBytes = new byte[ReadResponsesBytes.Length + responseBytes.Length];

                // TODO ---> @JonBianco BlockCopy should be leveraging here as it is vastly superior to Array.Copy
                // Combine prior bytes with new bytes
                Array.Copy(ReadResponsesBytes, 0, combinedResponseBytes, 0, ReadResponsesBytes.Length);
                Array.Copy(responseBytes, 0, combinedResponseBytes, ReadResponsesBytes.Length, responseBytes.Length);

                // Attempt to parse first message in response buffer
                var consumedResponseBytes = 0;
                var responseCode = 0;
                var errorFound = false;

                ReadErrorLevel readErrorLevel = ReadErrorLevel.None;

                // Validate NAD, PCB, and LEN values
                if (combinedResponseBytes.Length < 4)
                {
                    errorFound = true;
                    readErrorLevel = ReadErrorLevel.Length;
                }
                else if (!validNADValues.Contains(combinedResponseBytes[0]))
                {
                    errorFound = true;
                    readErrorLevel = ReadErrorLevel.Invalid_NAD;
                }
                else if (!validPCBValues.Contains(combinedResponseBytes[1]))
                {
                    errorFound = true;
                    readErrorLevel = ReadErrorLevel.Invalid_PCB;
                }
                else if (combinedResponseBytes[2] > (combinedResponseBytes.Length - (3 + 1)))
                {
                    errorFound = true;
                    readErrorLevel = ReadErrorLevel.Invalid_CombinedBytes;
                }
                else
                {
                    // Validate LRC
                    byte lrc = 0;
                    var index = 0;
                    for (index = 0; index < (combinedResponseBytes[2] + 3); index++)
                    {
                        lrc ^= combinedResponseBytes[index];
                    }

                    if (combinedResponseBytes[combinedResponseBytes[2] + 3] != lrc)
                    {
                        errorFound = true;
                        readErrorLevel = ReadErrorLevel.Missing_LRC;
                    }
                    else if ((combinedResponseBytes[1] & 0x01) == 0x01)
                    {
                        var componentBytes = new byte[combinedResponseBytes[2]];
                        Array.Copy(combinedResponseBytes, 3, componentBytes, 0, combinedResponseBytes[2]);
                        ReadResponseComponentBytes.Add(componentBytes);
                        consumedResponseBytes = combinedResponseBytes[2] + 3 + 1;
                        errorFound = true;
                        readErrorLevel = ReadErrorLevel.CombinedBytes_MisMatch;
                        addedResponseComponent = true;
                    }
                    else
                    {
                        var sw1Offset = combinedResponseBytes[2] + 3 - 2;
                        //if ((combinedResponseBytes[sw1Offset] != 0x90) && (combinedResponseBytes[sw1Offset + 1] != 0x00))
                        //    errorFound = true;
                        responseCode = (combinedResponseBytes[sw1Offset] << 8) + combinedResponseBytes[sw1Offset + 1];
                    }
                }

                if (!errorFound)
                {
                    var totalDecodeSize = combinedResponseBytes[2] - 2;        // Use LEN of final response packet
                    foreach (var component in ReadResponseComponentBytes)
                    {
                        totalDecodeSize += component.Length;
                    }

                    var totalDecodeBytes = new byte[totalDecodeSize];
                    var totalDecodeOffset = 0;
                    foreach (var component in ReadResponseComponentBytes)
                    {
                        Array.Copy(component, 0, totalDecodeBytes, totalDecodeOffset, component.Length);
                        totalDecodeOffset += component.Length;
                    }
                    Array.Copy(combinedResponseBytes, 3, totalDecodeBytes, totalDecodeOffset, combinedResponseBytes[2] - 2);    // Skip final response header and use LEN of final response (no including the SW1, SW2, and LRC)

                    ReadResponseComponentBytes = new List<byte[]>();

                    if (ResponseTagsHandler != null || ResponseContactlessHandler != null)
                    {
                        TLV.TLV tlv = new TLV.TLV();
                        var tags = tlv.Decode(totalDecodeBytes, 0, totalDecodeBytes.Length, nestedTagTags);

                        //PrintTags(tags);
                        if (ResponseTagsHandler != null)
                        {
                            ResponseTagsHandler.Invoke(tags, responseCode);
                        }
                        else if (ResponseContactlessHandler != null)
                        {
                            ResponseContactlessHandler.Invoke(tags, responseCode, combinedResponseBytes[1]);
                        }
                    }
                    else if (ResponseTaglessHandler != null)
                    {
                        ResponseTaglessHandler.Invoke(totalDecodeBytes, responseCode);
                    }

                    consumedResponseBytes = combinedResponseBytes[2] + 3 + 1;  // Consumed NAD, PCB, LEN, [LEN] bytes, and LRC

                    addedResponseComponent = (combinedResponseBytes.Length - consumedResponseBytes) > 0;
                }
                else
                {
                    // allows for debugging of VIPA read issues
                    System.Diagnostics.Debug.WriteLine($"VIPA-READ: ERROR LEVEL: '{readErrorLevel}'");
                }

                // Remove consumed bytes and leave remaining bytes for later consumption
                var remainingResponseBytes = new byte[combinedResponseBytes.Length - consumedResponseBytes];
                Array.Copy(combinedResponseBytes, consumedResponseBytes, remainingResponseBytes, 0, combinedResponseBytes.Length - consumedResponseBytes);

                ReadResponsesBytes = remainingResponseBytes;
            }

            if (addedResponseComponent)
            {
                ReadResponses(Array.Empty<byte>());
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

        public void WriteSingleCmd(VIPAResponseHandlers responsehandlers, VIPACommand command)
        {
            if (command == null)
            {
                return;
            }

            ResponseTagsHandler = responsehandlers.responsetagshandler;
            ResponseTaglessHandler = responsehandlers.responsetaglesshandler;
            ResponseContactlessHandler = responsehandlers.responsecontactlesshandler;

            int dataLen = command.data?.Length ?? 0;
            byte lrc = 0;

            if (0 < dataLen)
            {
                dataLen++;  // Allow for Lc byte
            }

            if (command.includeLE)
            {
                dataLen++;  // Allow for Le byte
            }

            var cmdLength = 7 /*NAD, PCB, LEN, CLA, INS, P1, P2*/ + dataLen + 1 /*LRC*/;
            var cmdBytes = new byte[cmdLength];
            var cmdIndex = 0;

            cmdBytes[cmdIndex++] = command.nad;
            lrc ^= command.nad;
            cmdBytes[cmdIndex++] = command.pcb;
            lrc ^= command.pcb;
            cmdBytes[cmdIndex++] = (byte)(4 /*CLA, INS, P1, P2*/ + dataLen /*Lc, data.Length, Le*/);
            lrc ^= (byte)(4 /*CLA, INS, P1, P2*/ + dataLen /*Lc, data.Length, Le*/);
            cmdBytes[cmdIndex++] = command.cla;
            lrc ^= command.cla;
            cmdBytes[cmdIndex++] = command.ins;
            lrc ^= command.ins;
            cmdBytes[cmdIndex++] = command.p1;
            lrc ^= command.p1;
            cmdBytes[cmdIndex++] = command.p2;
            lrc ^= command.p2;

            if (0 < command.data?.Length)
            {
                cmdBytes[cmdIndex++] = (byte)command.data.Length;
                lrc ^= (byte)command.data.Length;

                foreach (var byt in command.data)
                {
                    cmdBytes[cmdIndex++] = byt;
                    lrc ^= byt;
                }
            }

            if (command.includeLE)
            {
                cmdBytes[cmdIndex++] = command.le;
                lrc ^= command.le;
            }

            cmdBytes[cmdIndex++] = lrc;

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"VIPA-WRITE: {BitConverter.ToString(cmdBytes)}");
#endif
            WriteBytes(cmdBytes);
        }

        #endregion --- COMMANDS ---
    }
}
