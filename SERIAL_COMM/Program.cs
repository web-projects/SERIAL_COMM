using SERIAL_COMM.CommandLayer;
using SERIAL_COMM.Connection;
using System;
using System.Threading;

namespace SERIAL_COMM
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("main: connecting...");

            DeviceManager manager = new DeviceManager("COM3");

            if (manager.Connect())
            {
                Console.WriteLine("main: connected");

                while (manager.Connected())
                {
                    Thread.Sleep(5000);
                    Console.WriteLine("main: serial write...");

                    int timeout = 3000;
                    var cancelTokenSource = new CancellationTokenSource(timeout); ;

                    manager.WriteCommand(ReadCommands.DEVICE_RESET, cancelTokenSource, timeout);

                    cancelTokenSource.Cancel();
                    cancelTokenSource.Dispose();
                }

                Console.WriteLine("port: disconnected");
            }
        }
    }
}
