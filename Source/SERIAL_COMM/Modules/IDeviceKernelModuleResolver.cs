using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Text;

namespace SERIAL_COMM.Modules
{
    public interface IDeviceKernelModuleResolver
    {
        IKernel ResolveKernel(params NinjectModule[] modules);
    }
}
