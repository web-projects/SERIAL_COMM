using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using StateMachine.Tests;
using Moq;
using System;
using Xunit;

namespace StateMachine.State.Actions.Tests
{
    public class DeviceRecoveryStateActionTest : IDisposable
    {
        readonly DeviceRecoveryStateAction subject;
        readonly Mock<IDeviceStateController> mockController;
        readonly DeviceStateMachineAsyncManager asyncManager;

        public DeviceRecoveryStateActionTest()
        {
            mockController = new Mock<IDeviceStateController>();

            subject = new DeviceRecoveryStateAction(mockController.Object);

            asyncManager = new DeviceStateMachineAsyncManager(ref mockController, subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void WorkflowStateType_Should_Equal_DeviceRecovery()
            => Assert.Equal(DeviceWorkflowState.DeviceRecovery, subject.WorkflowStateType);

        [Fact]
        public async void DoWork_ShouldComplete_WhenCalled()
        {
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            mockController.Verify(e => e.Complete(subject));
        }
    }
}
