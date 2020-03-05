using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.SubWorkflows;
using DEVICE_CORE.StateMachine.State.Visitors;

namespace DEVICE_CORE.StateMachine.State.Providers
{
    internal interface IControllerVisitorProvider
    {
        IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundarySetupVisitor();
        IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundaryTeardownVisitor();
    }
}
