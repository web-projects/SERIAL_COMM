using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;
using StateMachine.State.Visitors;

namespace StateMachine.State.Providers
{
    internal class ControllerVisitorProvider : IControllerVisitorProvider
    {
        public IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundarySetupVisitor()
            => new WorkflowBoundarySetupVisitor();

        public IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundaryTeardownVisitor()
            => new WorkflowBoundaryTeardownVisitor();
    }
}
