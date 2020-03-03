﻿using Moq;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using Xunit;

namespace DEVICE_CORE.Tests.State.Actions
{
    public class DeviceManageStateActionTest : IDisposable
    {
        readonly DeviceManageStateAction subject;
        readonly Mock<IDeviceStateController> mockController;

        readonly DeviceStateMachineAsyncManager asyncManager;

        public DeviceManageStateActionTest()
        {
            mockController = new Mock<IDeviceStateController>();

            subject = new DeviceManageStateAction(mockController.Object);

            asyncManager = new DeviceStateMachineAsyncManager(ref mockController, subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void WorkflowStateType_Should_Equal_Manage()
            => Assert.Equal(DeviceWorkflowState.Manage, subject.WorkflowStateType);

        [Fact]
        public void RequestReceived_ShouldCallSaveState_When_Called()
        {
            //LinkRequest linkRequest = new LinkRequest();
            Object linkRequest = new object();
            subject.RequestReceived(linkRequest);

            Assert.True(asyncManager.WaitFor());

            mockController.Verify(e => e.Complete(subject), Times.Once());
            mockController.Verify(e => e.SaveState(linkRequest), Times.Once());
        }

        [Fact]
        public void DoDeviceDiscovery_ShouldErrorAndReturnTrue_WhenCalled()
        {
            bool expectedValue = subject.DoDeviceDiscovery();

            Assert.True(asyncManager.WaitFor());

            Assert.True(expectedValue);
            Assert.Equal("device recovery is needed", subject.LastException.Message);
            mockController.Verify(e => e.Error(subject), Times.Once());
        }
    }
}