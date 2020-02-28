using SERIAL_COMM.State.Enums;
using static SERIAL_COMM.State.Enums.SMWorkflowState;

namespace SERIAL_COMM.State
{
    public static class SMStateTransitionHelper
    {
        private static SMWorkflowState ComputeNoneStateTransition(bool exception) =>
            exception switch
            {
                true => InitializeDeviceCommunication,
                false => InitializeDeviceCommunication
            };

        private static SMWorkflowState ComputeDeviceRecoveryStateTransition(bool exception) =>
            exception switch
            {
                true => InitializeDeviceCommunication,
                false => InitializeDeviceCommunication
            };

        private static SMWorkflowState ComputeManageStateTransition(bool exception) =>
            exception switch
            {
                true => DeviceRecovery,
                false => ProcessRequest
            };

        private static SMWorkflowState ComputeProcessRequestStateTransition(bool exception) =>
            exception switch
            {
                true => SubWorkflowIdleState,
                false => SubWorkflowIdleState
            };

        private static SMWorkflowState ComputeSubWorkflowIdleStateTransition(bool exception) => 
            exception switch
            {
                true => DeviceRecovery,
                false => Manage
            };

        public static SMWorkflowState GetNextState(SMWorkflowState state, bool exception) =>
            state switch
            {
                None => ComputeNoneStateTransition(exception),
                DeviceRecovery => ComputeDeviceRecoveryStateTransition(exception),
                Manage => ComputeManageStateTransition(exception),
                ProcessRequest => ComputeProcessRequestStateTransition(exception),
                SubWorkflowIdleState => ComputeSubWorkflowIdleStateTransition(exception),
                _ => throw new StateException($"Invalid state transition '{state}' requested.")
            };
    }
}
