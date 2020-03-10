using StateMachine.State.Actions.SubWorkflows.Tests;
using StateMachine.State.Enums;
using Moq;
using Ninject;
using System;
using System.Threading.Tasks;
using XO.Requests;
using Xunit;

namespace StateMachine.State.SubWorkflows.Actions.Tests
{
    public class DeviceRequestSubCompleteStateActionTests : IDisposable
    {
        readonly IKernel kernel;
        readonly DeviceRequestCompleteSubStateAction subject;

        readonly Mock<IDeviceSubStateController> mockController;
        //readonly Mock<IListenerConnector> mockConnector;
        //readonly Mock<ILoggingServiceClient> mockLoggingClient;

        readonly LinkRequest linkRequest;

        readonly DeviceSubStateMachineAsyncManager asyncManager;

        public DeviceRequestSubCompleteStateActionTests()
        {
            linkRequest = new LinkRequest()
            {
                //LinkObjects = new LinkRequestIPA5Object
                //{
                //    LinkActionResponseList = new List<XO.Responses.LinkActionResponse>()
                //},
                //Actions = new List<LinkActionRequest>()
                //{
                //    new LinkActionRequest()
                //    {
                //        MessageID = Helpers.RandomGenerator.BuildRandomString(8),
                //        Timeout = 10000
                //    }
                //}
            };
            //linkRequest.LinkObjects = SampleBuilder.LinkObjectsForEvents();
            //linkRequest.LinkObjects.LinkActionResponseList[0].MessageID = "Authentication";
            //linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices = new List<LinkDeviceResponse>()
            //{
            //    new LinkDeviceResponse()
            //    {
            //        Manufacturer = "NoDevice",
            //        CardWorkflowControls = new LinkCardWorkflowControls()
            //        {
            //            CardCaptureTimeout = 10,
            //            ManualCardTimeout = 5
            //        }
            //    }
            //};

            //mockLoggingClient = new Mock<ILoggingServiceClient>();

            //mockConnector = new Mock<IListenerConnector>();
            mockController = new Mock<IDeviceSubStateController>();
            //mockController.SetupGet(e => e.LoggingClient).Returns(mockLoggingClient.Object);
            //mockController.SetupGet(e => e.Connector).Returns(mockConnector.Object);

            subject = new DeviceRequestCompleteSubStateAction(mockController.Object);

            asyncManager = new DeviceSubStateMachineAsyncManager(ref mockController, subject);

            using (kernel = new StandardKernel())
            {
                kernel.Bind<DeviceGetStatusSubStateAction>().ToSelf();
                kernel.Inject(subject);
            }
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void SubWorkflowStateType_Should_Equal_RequestComplete()
                    => Assert.Equal(DeviceSubWorkflowState.RequestComplete, subject.WorkflowStateType);

        [Fact]
        public void DoWork_ShouldSimplyComplete_When_NoStateObjectProvided()
        {
            Assert.Equal(subject.DoWork(), Task.CompletedTask);

            Assert.True(asyncManager.WaitFor());
        }

        [Fact]
        public void DoWork_ShouldDoWorkAndComplete_When_StateObjectIsProvided()
        {
            subject.SetState(linkRequest);

            Assert.Equal(subject.DoWork(), Task.CompletedTask);

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogInfoAsync(It.IsAny<string>(), null), Times.Once());
            //mockConnector.Verify(e => e.Publish(It.IsAny<object>(), new TopicOption[] { TopicOption.Servicer }), Times.Once());
            mockController.Verify(e => e.Complete(subject), Times.Once());
        }
    }
}
