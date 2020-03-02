using SERIAL_COMM.StateMachine.State;
using SERIAL_COMM.StateMachine.State.Enums;
using System;
using Xunit;

namespace SERIAL_COMM.Tests.State
{
    public class DeviceStateTransitionHelperTest
    {
        [Theory]
        [InlineData(DeviceWorkflowState.None, DeviceWorkflowState.InitializeDeviceCommunication)]
        [InlineData(DeviceWorkflowState.None, DeviceWorkflowState.InitializeDeviceCommunication, true)]
        [InlineData(DeviceWorkflowState.DeviceRecovery, DeviceWorkflowState.InitializeDeviceCommunication)]
        [InlineData(DeviceWorkflowState.DeviceRecovery, DeviceWorkflowState.InitializeDeviceCommunication, true)]
        [InlineData(DeviceWorkflowState.InitializeDeviceCommunication, DeviceWorkflowState.InitializeDeviceHealth)]
        [InlineData(DeviceWorkflowState.InitializeDeviceCommunication, DeviceWorkflowState.DeviceRecovery, true)]
        [InlineData(DeviceWorkflowState.InitializeDeviceHealth, DeviceWorkflowState.Manage)]
        [InlineData(DeviceWorkflowState.InitializeDeviceHealth, DeviceWorkflowState.DeviceRecovery, true)]
        [InlineData(DeviceWorkflowState.Manage, DeviceWorkflowState.ProcessRequest)]
        [InlineData(DeviceWorkflowState.Manage, DeviceWorkflowState.DeviceRecovery, true)]
        //[InlineData(DeviceWorkflowState.ProcessRequest, DeviceWorkflowState.SubWorkflowIdleState)]
        //[InlineData(DeviceWorkflowState.ProcessRequest, DeviceWorkflowState.SubWorkflowIdleState, true)]
        //[InlineData(DeviceWorkflowState.SubWorkflowIdleState, DeviceWorkflowState.Manage)]
        //[InlineData(DeviceWorkflowState.SubWorkflowIdleState, DeviceWorkflowState.DeviceRecovery, true)]
        public void GetNextState_ShouldReturnExpectedNextState_When_Called(DeviceWorkflowState currentState, DeviceWorkflowState expectedState, bool exceptionState = false)
        {
            Func<DeviceWorkflowState, bool, DeviceWorkflowState> method = (DeviceWorkflowState state, bool exception)
                => DeviceStateTransitionHelper.GetNextState(state, exception);

            Assert.Equal(expectedState, method(currentState, exceptionState));
        }
    }
}
