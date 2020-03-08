using Moq;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using Xunit;
using DEVICE_CORE.StateMachine.Tests;

namespace DEVICE_CORE.StateMachine.State.Actions.Tests
{
    public class DeviceShutdownStateActionTest : IDisposable
    {
        readonly DeviceShutdownStateAction subject;
        readonly Mock<IDeviceStateController> mockController;

        readonly DeviceStateMachineAsyncManager asyncManager;

        public DeviceShutdownStateActionTest()
        {
            mockController = new Mock<IDeviceStateController>();

            subject = new DeviceShutdownStateAction(mockController.Object);

            asyncManager = new DeviceStateMachineAsyncManager(ref mockController, subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void WorkflowStateType_Should_Equal_Shutdown()
            => Assert.Equal(DeviceWorkflowState.Shutdown, subject.WorkflowStateType);

        [Fact]
        public async void DoWork_ShouldCompleteAction_When_Called()
        {
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            mockController.Verify(e => e.Complete(subject));
        }
    }
}
