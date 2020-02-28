using Ninject.Modules;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Connection.SerialPort;
using System;
using System.Collections.Generic;
using System.Text;

namespace SERIAL_COMM.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISerialPortMonitor>().To<SerialPortMonitor>();
        }
    }
}
