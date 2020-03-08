using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;

namespace StateMachine.State.Visitors
{
    internal class WorkflowBoundaryTeardownVisitor : IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController>
    {
        public void Visit(ISubWorkflowHook context, IDeviceSubStateController visitorAcceptor) => context.UnHook();
    }
}
