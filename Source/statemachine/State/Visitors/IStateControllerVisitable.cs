using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;

namespace StateMachine.State.Visitors
{
    internal interface IStateControllerVisitable<TVisitableController, TVisitorAcceptor>
        where TVisitableController : ISubWorkflowHook
        where TVisitorAcceptor : IDeviceSubStateController
    {
        void Accept(IStateControllerVisitor<TVisitableController, TVisitorAcceptor> visitor);
    }
}
