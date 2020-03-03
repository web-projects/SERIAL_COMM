using Moq;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using Xunit;

namespace DEVICE_CORE.Tests.State.Actions
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
