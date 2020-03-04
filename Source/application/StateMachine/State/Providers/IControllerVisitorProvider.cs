using DEVICE_CORE.State.Interfaces;
using DEVICE_CORE.State.SubWorkflows;
using DEVICE_CORE.State.Visitors;

namespace DEVICE_CORE.State.Providers
{
    internal interface IControllerVisitorProvider
    {
        IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundarySetupVisitor();
        IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundaryTeardownVisitor();
    }
}
