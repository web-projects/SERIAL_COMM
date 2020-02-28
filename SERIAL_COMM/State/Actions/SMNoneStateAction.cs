using IPA5.DAL.Core.State;
using IPA5.DAL.Core.State.Actions;
using SERIAL_COMM.State.Enums;
using System.Threading.Tasks;

namespace SERIAL_COMM.State.Actions
{
    internal class SMNoneStateAction : SMBaseStateAction
    {
        public override SMWorkflowState WorkflowStateType => SMWorkflowState.None;

        public SMNoneStateAction(ISMStateController _) : base(_) { }

        public override Task DoWork()
        {
            _ = Complete(this);

            return Task.CompletedTask;
        }
    }
}
