using SERIAL_COMM.Cancellation;
using SERIAL_COMM.CommandLayer;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;

namespace SERIAL_COMM
{
    class Program
    {
        const int COMMAND_TIMEOUT = 3;
        static public CancellationToken CancellationToken { get; private set; }

        static void Main(string[] args)
        {
            Console.WriteLine($"\r\n==========================================================================================");
            Console.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name} - Version {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"==========================================================================================\r\n");

            if (args.Length == 1)
            {
                string comPort = args[0];
                Regex rgx = new Regex(@"\d+");
                if (comPort.IndexOf("COM") == 0 && rgx.IsMatch(comPort))
                {
                    Console.WriteLine("main: connecting...");
                    ProcessCommand(comPort);
                }
                else
                {
                    Console.WriteLine($"Invalid parameter given '{comPort}' - should be like 'COM#' where '#' is a number.");
                }
            }
            else
            {
                Console.WriteLine($"Missing COM parameter - [COMX]");
            }
        }

        static async void ProcessCommand(string comPort)
        {
            DeviceManager manager = new DeviceManager(comPort);

            if (manager.Connect())
            {
                Console.WriteLine("main: connected");

                while (manager.Connected())
                {
                    Thread.Sleep(5000);
                    Console.WriteLine("main: serial write...");

                    IDeviceCancellationBroker cancellationBroker = manager.GetDeviceCancellationBroker();

                    Object output = new object();
                    var timeoutPolicy = await cancellationBroker.ExecuteWithTimeoutAsync<Object>(
                                        _ => manager.WriteCommand(ReadCommands.DEVICE_RESET),
                                        COMMAND_TIMEOUT, CancellationToken);

                    if (timeoutPolicy.Outcome == Polly.OutcomeType.Failure)
                    {
                        Console.WriteLine($"Unable to obtain Card Data from device.");
                    }
                }

                Console.WriteLine("port: disconnected");
            }
        }

        public void SetCancellationToken(CancellationToken cancellationToken) => (CancellationToken) = (cancellationToken);
    }
}
