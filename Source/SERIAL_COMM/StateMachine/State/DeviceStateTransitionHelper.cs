using SERIAL_COMM.StateMachine.State.Enums;
using static SERIAL_COMM.StateMachine.State.Enums.DeviceWorkflowState;

namespace SERIAL_COMM.StateMachine.State
{
    public static class DeviceStateTransitionHelper
    {
        private static DeviceWorkflowState ComputeNoneStateTransition(bool exception) =>
            exception switch
            {
                //true => InitializeDeviceCommunication,
                //false => InitializeDeviceCommunication
                true => Manage,
                false => Manage
            };

        private static DeviceWorkflowState ComputeDeviceRecoveryStateTransition(bool exception) =>
            exception switch
            {
                true => InitializeDeviceCommunication,
                false => InitializeDeviceCommunication
            };

        private static DeviceWorkflowState ComputeManageStateTransition(bool exception) =>
            exception switch
            {
                true => DeviceRecovery,
                false => ProcessRequest
            };

        private static DeviceWorkflowState ComputeProcessRequestStateTransition(bool exception) =>
            exception switch
            {
                true => SubWorkflowIdleState,
                false => SubWorkflowIdleState
            };

        private static DeviceWorkflowState ComputeSubWorkflowIdleStateTransition(bool exception) => 
            exception switch
            {
                true => DeviceRecovery,
                false => Manage
            };

        public static DeviceWorkflowState GetNextState(DeviceWorkflowState state, bool exception) =>
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
