using DEVICE_CORE.StateMachine.State.Enums;
using static DEVICE_CORE.StateMachine.State.Enums.DeviceWorkflowState;

namespace DEVICE_CORE.StateMachine.State
{
    public static class DeviceStateTransitionHelper
    {
        private static DeviceWorkflowState ComputeNoneStateTransition(bool exception) =>
            exception switch
            {
                true => InitializeDeviceCommunication,
                false => InitializeDeviceCommunication
            };

        private static DeviceWorkflowState ComputeDeviceRecoveryStateTransition(bool exception) =>
            exception switch
            {
                true => InitializeDeviceCommunication,
                false => InitializeDeviceCommunication
            };

        private static DeviceWorkflowState ComputeInitializeDeviceCommunicationStateTransition(bool exception) =>
            exception switch
            {
                true => DeviceRecovery,
                false => InitializeDeviceHealth
            };

        private static DeviceWorkflowState ComputeInitializeDeviceHealthStateTransition(bool exception) =>
            exception switch
            {
                true => DeviceRecovery,
                false => Manage
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
                InitializeDeviceCommunication => ComputeInitializeDeviceCommunicationStateTransition(exception),
                InitializeDeviceHealth => ComputeInitializeDeviceHealthStateTransition(exception),
                Manage => ComputeManageStateTransition(exception),
                ProcessRequest => ComputeProcessRequestStateTransition(exception),
                SubWorkflowIdleState => ComputeSubWorkflowIdleStateTransition(exception),
                _ => throw new StateException($"Invalid state transition '{state}' requested.")
            };
    }
}
