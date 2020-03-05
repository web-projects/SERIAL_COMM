using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.SubWorkflows;
using DEVICE_CORE.StateMachine.State.Visitors;

namespace DEVICE_CORE.StateMachine.State.Providers
{
    internal class ControllerVisitorProvider : IControllerVisitorProvider
    {
        public IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundarySetupVisitor()
            => new WorkflowBoundarySetupVisitor();

        public IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController> CreateBoundaryTeardownVisitor()
            => new WorkflowBoundaryTeardownVisitor();
    }
}
