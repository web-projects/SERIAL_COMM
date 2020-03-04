using DEVICE_CORE.StateMachine.State.Enums;
using System;
using System.Linq;
using XO.Requests;

namespace DEVICE_CORE.State.SubWorkflows.Providers
{
    internal class InitialStateProvider
    {
        public DeviceSubWorkflowState DetermineInitialState(LinkRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            LinkActionRequest linkActionRequest = request.Actions.First();
            DeviceSubWorkflowState proposedState = ((linkActionRequest.DeviceActionRequest?.DeviceAction) switch
            {
                LinkDeviceActionType.AbortCommand => DeviceSubWorkflowState.AbortCommand,
                LinkDeviceActionType.ResetCommand => DeviceSubWorkflowState.ResetCommand,
                _ => DeviceSubWorkflowState.Undefined
            });

            return proposedState;
        }
    }
}
