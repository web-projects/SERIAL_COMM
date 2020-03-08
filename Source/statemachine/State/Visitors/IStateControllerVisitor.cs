using StateMachine.State.Interfaces;

namespace StateMachine.State.Visitors
{
    internal interface IStateControllerVisitor<TVisitableController, TVisitorAcceptor> where TVisitableController : ISubWorkflowHook
    {
        void Visit(TVisitableController context, TVisitorAcceptor visitorAcceptor);
    }
}
