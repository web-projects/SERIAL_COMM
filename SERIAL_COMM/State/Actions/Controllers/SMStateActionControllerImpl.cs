using IPA5.DAL.Core.State;
using SERIAL_COMM.State.Enums;
using SERIAL_COMM.State.Management;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SERIAL_COMM.State.Actions.Controllers
{
    internal class SMStateActionControllerImpl : ISMStateActionController
    {
        private readonly ISMStateManager manager;
        private readonly ReadOnlyDictionary<SMWorkflowState, Func<ISMStateController, ISMStateAction>> workflowMap =
            new ReadOnlyDictionary<SMWorkflowState, Func<ISMStateController, ISMStateAction>>(
                new Dictionary<SMWorkflowState, Func<ISMStateController, ISMStateAction>>
                {
                    [SMWorkflowState.None] = (ISMStateController _) => new SMNoneStateAction(_),
                    //[SMWorkflowState.DeviceRecovery] = (ISMStateController _) => new SMDeviceRecoveryStateAction(_),
                    //[SMWorkflowState.InitializeDeviceCommunication] = (ISMStateController _) => new SMInitializeDeviceCommunicationStateAction(_),
                    //[SMWorkflowState.Manage] = (ISMStateController _) => new SMManageStateAction(_),
                    //[SMWorkflowState.ProcessRequest] = (ISMStateController _) => new SMProcessRequestStateAction(_),
                    //[SMWorkflowState.SubWorkflowIdleState] = (ISMStateController _) => new SMSubWorkflowIdleStateAction(_),
                    //[SMWorkflowState.Shutdown] = (ISMStateController _) => new SMShutdownStateAction(_)
                }
        );

        private ISMStateAction currentStateAction;

        public SMStateActionControllerImpl(ISMStateManager manager) => (this.manager) = (manager);

        public ISMStateAction GetFinalState()
            => workflowMap[SMWorkflowState.Shutdown](manager as ISMStateController);

        public ISMStateAction GetNextAction(ISMStateAction stateAction)
            => GetNextAction(stateAction.WorkflowStateType);

        public ISMStateAction GetNextAction(SMWorkflowState state)
        {
            ISMStateController controller = manager as ISMStateController;
            if (currentStateAction == null)
            {
                return (currentStateAction = workflowMap[SMWorkflowState.None](controller));
            }

            SMWorkflowState proposedState = SMStateTransitionHelper.GetNextState(state, currentStateAction.LastException != null);

            if (proposedState == currentStateAction.WorkflowStateType)
            {
                return currentStateAction;
            }

            return (currentStateAction = workflowMap[proposedState](controller));
        }

        public ISMStateAction GetSpecificAction(SMWorkflowState proposedState)
        {
            ISMStateController controller = manager as ISMStateController;

            return (currentStateAction = workflowMap[proposedState](controller));
        }
    }
}
