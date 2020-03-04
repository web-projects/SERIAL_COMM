using System;
using System.Linq;

namespace DEVICE_CORE.State.SubWorkflows.Providers
{
    internal class InitialStateProvider
    {
        public DeviceSubWorkflowState DetermineInitialState(object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            object linkActionRequest = request;
            DeviceSubWorkflowState proposedState = ((linkActionRequest.DeviceActionRequest?.DeviceAction) switch
            {
                LinkDALActionType.DeviceAbort => DeviceAbort,
                LinkDALActionType.DeviceReset => DeviceReset,
                _ => Undefined
            });

            return proposedState;
        }
    }
}
