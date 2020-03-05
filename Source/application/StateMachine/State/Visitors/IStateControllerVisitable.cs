using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.SubWorkflows;

namespace DEVICE_CORE.StateMachine.State.Visitors
{
    internal interface IStateControllerVisitable<TVisitableController, TVisitorAcceptor>
        where TVisitableController : ISubWorkflowHook
        where TVisitorAcceptor : IDeviceSubStateController
    {
        void Accept(IStateControllerVisitor<TVisitableController, TVisitorAcceptor> visitor);
    }
}
