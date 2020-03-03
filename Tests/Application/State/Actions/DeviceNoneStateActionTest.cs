using Moq;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using Xunit;

namespace DEVICE_CORE.Tests.State.Actions
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
