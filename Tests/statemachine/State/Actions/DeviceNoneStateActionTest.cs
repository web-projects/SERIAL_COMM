using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using StateMachine.Tests;
using Moq;
using System;
using Xunit;

namespace StateMachine.State.Actions.Tests
{
    public class DeviceNoneStateActionTest : IDisposable
    {
        readonly DeviceNoneStateAction subject;
        readonly Mock<IDeviceStateController> mockController;

        readonly DeviceStateMachineAsyncManager asyncManager;

        public DeviceNoneStateActionTest()
        {
            mockController = new Mock<IDeviceStateController>();

            subject = new DeviceNoneStateAction(mockController.Object);

            asyncManager = new DeviceStateMachineAsyncManager(ref mockController, subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void WorkflowStateType_Should_Equal_None()
            => Assert.Equal(DeviceWorkflowState.None, subject.WorkflowStateType);

        [Fact]
        public void DoWork_ShouldCompleteAction_When_Called()
        {
            subject.DoWork().Wait(2000);

            Assert.True(asyncManager.WaitFor());

            mockController.Verify(e => e.Complete(subject));
        }
    }
}
