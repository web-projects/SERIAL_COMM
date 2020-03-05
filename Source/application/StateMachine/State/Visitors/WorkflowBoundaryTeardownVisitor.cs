using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.SubWorkflows;

namespace DEVICE_CORE.StateMachine.State.Visitors
{
    internal class WorkflowBoundaryTeardownVisitor : IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController>
    {
        public void Visit(ISubWorkflowHook context, IDeviceSubStateController visitorAcceptor) => context.UnHook();
    }
}
