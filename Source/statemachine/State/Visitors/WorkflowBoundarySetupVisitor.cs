using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;

namespace StateMachine.State.Visitors
{
    internal class WorkflowBoundarySetupVisitor : IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController>
    {
        public void Visit(ISubWorkflowHook context, IDeviceSubStateController visitorAcceptor) => context.Hook(visitorAcceptor);
    }
}
