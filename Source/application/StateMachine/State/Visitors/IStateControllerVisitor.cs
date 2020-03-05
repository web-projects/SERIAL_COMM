using DEVICE_CORE.StateMachine.State.Interfaces;

namespace DEVICE_CORE.StateMachine.State.Visitors
{
    internal interface IStateControllerVisitor<TVisitableController, TVisitorAcceptor> where TVisitableController : ISubWorkflowHook
    {
        void Visit(TVisitableController context, TVisitorAcceptor visitorAcceptor);
    }
}
