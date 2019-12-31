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

            SerialConnection connection = new SerialConnection("COM3");

            if (connection.Connect())
            {
                Console.WriteLine("main: connected");

                while (connection.Connected())
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("main: serial read...");
                }

                Console.WriteLine("port: disconnected");
            }
        }
    }
}
