using SERIAL_COMM.StateMachine.State;
using SERIAL_COMM.StateMachine.State.Enums;
using System;
using Xunit;

namespace SERIAL_COMM.Tests.State
{
    public class DeviceStateTransitionHelperTest
    {
        [Theory]
        //[InlineData(None, InitializeDeviceCommunication)]
        //[InlineData(None, InitializeDeviceCommunication, true)]
        //[InlineData(DeviceRecovery, InitializeDeviceCommunication)]
        //[InlineData(DeviceRecovery, InitializeDeviceCommunication, true)]
        //[InlineData(InitializeDeviceCommunication, InitializeDeviceHealth)]
        //[InlineData(InitializeDeviceCommunication, DeviceRecovery, true)]
        //[InlineData(InitializeDeviceHealth, ConnectToServer)]
        //[InlineData(InitializeDeviceHealth, DeviceRecovery, true)]
        //[InlineData(ConnectToServer, Manage)]
        //[InlineData(ConnectToServer, AttemptServerReconnect, true)]
        [InlineData(DeviceWorkflowState.Manage, DeviceWorkflowState.ProcessRequest)]
        [InlineData(DeviceWorkflowState.Manage, DeviceWorkflowState.DeviceRecovery, true)]
        //[InlineData(ProcessRequest, SubWorkflowIdleState)]
        //[InlineData(ProcessRequest, SubWorkflowIdleState, true)]
        //[InlineData(SubWorkflowIdleState, Manage)]
        //[InlineData(SubWorkflowIdleState, DeviceRecovery, true)]
        public void GetNextState_ShouldReturnExpectedNextState_When_Called(DeviceWorkflowState currentState, DeviceWorkflowState expectedState, bool exceptionState = false)
        {
            Func<DeviceWorkflowState, bool, DeviceWorkflowState> method = (DeviceWorkflowState state, bool exception)
                => DeviceStateTransitionHelper.GetNextState(state, exception);

            Assert.Equal(expectedState, method(currentState, exceptionState));
        }
    }
}
