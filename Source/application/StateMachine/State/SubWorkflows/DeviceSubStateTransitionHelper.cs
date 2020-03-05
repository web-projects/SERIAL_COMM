using DEVICE_CORE.StateMachine.State.Enums;

using static DEVICE_CORE.StateMachine.State.Enums.DeviceSubWorkflowState;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows
{
    public static class DeviceSubStateTransitionHelper
    {
        private static DeviceSubWorkflowState ComputeGetStatusStateTransition(bool exception) =>
            exception switch
            {
                true => SanityCheck,
                false => SanityCheck
            };

        private static DeviceSubWorkflowState ComputeDeviceAbortStateTransition(bool exception) =>
            exception switch
            {
                true => SanityCheck,
                false => SanityCheck
            };

        private static DeviceSubWorkflowState ComputeDeviceResetStateTransition(bool exception) =>
            exception switch
            {
                true => SanityCheck,
                false => SanityCheck
            };

        private static DeviceSubWorkflowState ComputeSanityCheckStateTransition(bool exception) =>
            exception switch
            {
                true => RequestComplete,
                false => RequestComplete
            };

        private static DeviceSubWorkflowState ComputeRequestCompletedStateTransition(bool exception) =>
            exception switch
            {
                true => Undefined,
                false => Undefined
            };

        public static DeviceSubWorkflowState GetNextState(DeviceSubWorkflowState state, bool exception) =>
            state switch
            {
                GetStatus => ComputeGetStatusStateTransition(exception),
                AbortCommand => ComputeDeviceAbortStateTransition(exception),
                ResetCommand => ComputeDeviceResetStateTransition(exception),
                SanityCheck => ComputeSanityCheckStateTransition(exception),
                RequestComplete => ComputeRequestCompletedStateTransition(exception),
                _ => throw new StateException($"Invalid state transition '{state}' requested.")
            };
    }
}
