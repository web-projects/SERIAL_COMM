using DEVICE_CORE.State.Interfaces;
using DEVICE_CORE.State.SubWorkflows;

namespace DEVICE_CORE.State.Visitors
{
    internal interface IStateControllerVisitable<TVisitableController, TVisitorAcceptor>
        where TVisitableController : ISubWorkflowHook
        where TVisitorAcceptor : IDeviceSubStateController
    {
        void Accept(IStateControllerVisitor<TVisitableController, TVisitorAcceptor> visitor);
    }
}
