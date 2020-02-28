using SERIAL_COMM.State.Enums;

namespace SERIAL_COMM.State.Actions.Controllers
{
    internal interface ISMStateActionController
    {
        ISMStateAction GetFinalState();
        ISMStateAction GetNextAction(ISMStateAction stateAction);
        ISMStateAction GetNextAction(SMWorkflowState state);
        ISMStateAction GetSpecificAction(SMWorkflowState state);
    }
}
