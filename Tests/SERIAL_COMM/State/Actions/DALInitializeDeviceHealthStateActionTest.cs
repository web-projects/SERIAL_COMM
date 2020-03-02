using Moq;
using SERIAL_COMM.StateMachine.State.Actions;
using SERIAL_COMM.StateMachine.State.Enums;
using SERIAL_COMM.StateMachine.State.Interfaces;
using SERIAL_COMM.Tests;
using System;
using Xunit;

namespace IPA5.Devices.DAL.Core.Tests.State.Actions
{
    public class DALInitializeDeviceHealthStateActionTest : IDisposable
    {
        readonly DeviceInitializeDeviceHealthStateAction subject;
        readonly Mock<IDeviceStateController> mockController;
        //readonly Mock<ILoggingServiceClient> mockLoggingClient;

        readonly DeviceStateMachineAsyncManager asyncManager;

        public DALInitializeDeviceHealthStateActionTest()
        {
            //mockLoggingClient = new Mock<ILoggingServiceClient>();

            mockController = new Mock<IDeviceStateController>();
            //mockController.SetupGet(e => e.LoggingClient).Returns(mockLoggingClient.Object);

            subject = new DeviceInitializeDeviceHealthStateAction(mockController.Object);

            asyncManager = new DeviceStateMachineAsyncManager(ref mockController, subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void WorkflowStateType_Should_Equal_InitializeDeviceHealth()
            => Assert.Equal(DeviceWorkflowState.InitializeDeviceHealth, subject.WorkflowStateType);

        [Fact]
        public async void DoWork_ShouldCallSetPublishEventHandlerAsTask_When_Called()
        {
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogInfoAsync(It.IsAny<string>(), null), Times.Once());

            //mockController.Verify(e => e.SetPublishEventHandlerAsTask(), Times.Once());
            mockController.Verify(e => e.Complete(subject));
        }
    }
}
