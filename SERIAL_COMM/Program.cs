﻿using Ninject;
using SERIAL_COMM.Cancellation;
using SERIAL_COMM.CommandLayer;
using SERIAL_COMM.CommandLayer.Helpers;
using SERIAL_COMM.CommandLayer.VIPA;
using System;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SERIAL_COMM
{
    class Program
    {
        const int WRITE_COMMAND_TIMEOUT = 3;
        static public CancellationToken CancellationToken { get; private set; }

        [Inject]
        internal SMStateManager SMStateManager { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine($"\r\n==========================================================================================");
            Console.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name} - Version {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"==========================================================================================\r\n");

            if (args.Length == 2)
            {
                string comPort = args[1];
                Regex rgx = new Regex(@"\d+");
                if (comPort.IndexOf("COM") == 0 && rgx.IsMatch(comPort))
                {
                    Console.WriteLine("main: connecting...");
                    switch (args[0].ToUpper())
                    {
                        case "/ABORT":
                        {
                            ProcessCommand(comPort, ReadCommands.DEVICE_ABORT);
                            break;
                        }
                        case "/RESET":
                        {
                            ProcessCommand(comPort, ReadCommands.DEVICE_RESET);
                            break;
                        }
                        default:
                        {
                            Console.WriteLine($"Invalid command given '{args[0].ToUpper()}' - valid: [/ABORT | /RESET]");
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Invalid parameter given '{comPort}' - should be like 'COM#' where '#' is a number.");
                }
            }
            else
            {
                Console.WriteLine($"Missing COM parameter(s) - [COMX][/ABORT | /RESET]");
            }
        }

        public Task Run()
        {
            _ = Task.Run(() => SMStateManager.LaunchWorkflow());
            return Task.CompletedTask;
        }

        static async void ProcessCommand(string comPort, ReadCommands readCommand)
        {
            DeviceManager manager = new DeviceManager(comPort);

            if (manager?.Connect() ?? false)
            {
                Console.WriteLine("main: connected");

                while (manager?.Connected() ?? false)
                {
                    Thread.Sleep(5000);
                    Console.WriteLine("main: serial write...");

                    IDeviceCancellationBroker cancellationBroker = manager.GetDeviceCancellationBroker();

                    (DeviceInfoObject deviceInfoObject, int VipaResponse) deviceResponse = (null, (int)VipaSW1SW2Codes.Failure);

                    var timeoutPolicy = await cancellationBroker.ExecuteWithTimeoutAsync<(DeviceInfoObject deviceInfoObject, int VipaResponse)>(
                                        _ => manager.WriteCommand(readCommand),
                                        WRITE_COMMAND_TIMEOUT, CancellationToken);

                    if (timeoutPolicy.Outcome == Polly.OutcomeType.Failure)
                    {
                        Console.WriteLine($"Unable to obtain response from device for command=[{readCommand}].");
                    }
                    else
                    {
                        string vipaResponse = string.Format("0x{0:X}", timeoutPolicy.Result.VipaResponse);
                        Console.WriteLine($"command: {readCommand} - VIPA RESPONSE={vipaResponse}");
                    }
                }

                Console.WriteLine("port: disconnected");
            }
        }

        public void SetCancellationToken(CancellationToken cancellationToken) => (CancellationToken) = (cancellationToken);
    }
}
