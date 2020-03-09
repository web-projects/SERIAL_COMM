using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DEVICE_CORE
{
    class Program
    {
        static readonly DeviceActivator activator = new DeviceActivator();

        static async Task Main(string[] args)
        {
            Console.WriteLine($"\r\n==========================================================================================");
            Console.WriteLine($"{Assembly.GetEntryAssembly().GetName().Name} - Version {Assembly.GetEntryAssembly().GetName().Version}");
            Console.WriteLine($"==========================================================================================\r\n");

            string pluginPath = Path.Combine(Environment.CurrentDirectory, "DevicePlugins");

            IDeviceApplication application = activator.Start(pluginPath);
            await application.Run().ConfigureAwait(false);

            Console.WriteLine("COMMANDS: [a=ABORT, r=RESET, q=QUIT]\r\n");

            ConsoleKey keypressed = Console.ReadKey(true).Key;

            while (keypressed != ConsoleKey.Q)
            {
                switch (keypressed)
                {
                    case ConsoleKey.A:
                    {
                        Console.WriteLine("\r\nCOMMAND: [ABORT]");
                        break;
                    }
                    case ConsoleKey.R:
                    {
                        Console.WriteLine("\r\nCOMMAND: [RESET]");
                        break;
                    }
                }

                await Task.Delay(50).ConfigureAwait(false);

                keypressed = Console.ReadKey(true).Key;
            }

            Console.WriteLine("\r\nCOMMAND: [QUIT]\r\n");

            application.Shutdown();
        }
    }
}
