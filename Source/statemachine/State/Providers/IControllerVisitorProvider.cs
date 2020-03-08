using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;
using StateMachine.State.Visitors;

namespace StateMachine.State.Providers
{
    internal interface IControllerVisitorProvider
    {
        IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundarySetupVisitor();
        IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundaryTeardownVisitor();
    }
}
