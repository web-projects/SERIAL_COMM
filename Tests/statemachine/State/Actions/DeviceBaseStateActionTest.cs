﻿using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace StateMachine.State.Actions.Tests
{
    public class DeviceBaseStateActionTest
    {
        readonly DeviceBaseStateActionFake subject;
        readonly Mock<IDeviceStateController> mockDeviceStateController;

        public DeviceBaseStateActionTest()
        {
            mockDeviceStateController = new Mock<IDeviceStateController>();
            subject = new DeviceBaseStateActionFake(mockDeviceStateController.Object);
        }

        [Fact]
        public void DoWork_ShouldReturnCompleteTask_When_Asked()
        {
            Assert.Equal(subject.DoWork(), Task.CompletedTask);
        }

        [Theory]
        [InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.None)]
        [InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.DeviceRecovery)]
        [InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.InitializeDeviceCommunication)]
        //[InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.InitializeDeviceHealth)]
        [InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.Manage)]
        [InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.ProcessRequest)]
        [InlineData(typeof(DeviceWorkflowState), DeviceWorkflowState.Shutdown)]
        public void SetState_ShouldSetState_When_Asked(Type expectedType, object stateObject)
        {
            subject.SetState(stateObject);
            Assert.Equal(stateObject, subject.StateObject);
            Assert.IsType(expectedType, subject.StateObject);
        }
    }

    class DeviceBaseStateActionFake : DeviceBaseStateAction
    {
        public DeviceBaseStateActionFake(IDeviceStateController _) : base(_) { }

        //public override DeviceWorkflowState SubWorkflowStateType => DeviceWorkflowState.Undefined;

        public override DeviceWorkflowState WorkflowStateType => throw new NotImplementedException();
    }
}
