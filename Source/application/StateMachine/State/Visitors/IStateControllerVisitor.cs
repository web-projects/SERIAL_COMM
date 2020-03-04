using DEVICE_CORE.State.Interfaces;

namespace DEVICE_CORE.State.Visitors
{
    internal interface IStateControllerVisitor<TVisitableController, TVisitorAcceptor> where TVisitableController : ISubWorkflowHook
    {
        void Visit(TVisitableController context, TVisitorAcceptor visitorAcceptor);
    }
}
