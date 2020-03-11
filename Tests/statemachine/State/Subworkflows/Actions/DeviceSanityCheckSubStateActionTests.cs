using StateMachine.Cancellation;
using StateMachine.State.Actions.SubWorkflows.Tests;
using StateMachine.State.Enums;
using Devices.Common;
using Devices.Common.Interfaces;
using Moq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestHelper.Polly;
using XO.Requests;
using Xunit;
using XO.Device;
using DEVICE_CORE.Helpers.Tests;

namespace StateMachine.State.SubWorkflows.Actions.Tests
{
    public class DeviceSanityCheckSubStateActionTests : IDisposable
    {
        readonly IKernel kernel;
        readonly DeviceSanityCheckSubStateAction subject;

        readonly Mock<IDeviceSubStateController> mockSubController;
        //readonly Mock<ILoggingServiceClient> mockLoggingClient;

        readonly Mock<IDeviceCancellationBroker> mockDeviceCancellationBroker;

        readonly LinkRequest linkRequest;

        readonly DeviceSubStateMachineAsyncManager asyncManager;

        public DeviceSanityCheckSubStateActionTests()
        {
            linkRequest = new LinkRequest()
            {
                //LinkObjects = new LinkRequestIPA5Object
                //{
                //    LinkActionResponseList = new List<XO.Responses.LinkActionResponse>()
                //},
                Actions = new List<LinkActionRequest>()
                {
                    new LinkActionRequest()
                    {
                        MessageID = RandomGenerator.BuildRandomString(8),
                        Timeout = 10000,
                        DeviceRequest = new LinkDeviceRequest()
                        {
                            DeviceIdentifier = new LinkDeviceIdentifier()
                            {
                                Manufacturer = "DeviceMockerInc",
                                Model = "DeviceMokerModel",
                                SerialNumber = "CEEEDEADBEEF"
                            }
                        }
                    }
                }
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

            mockDeviceCancellationBroker = new Mock<IDeviceCancellationBroker>();

            mockSubController = new Mock<IDeviceSubStateController>();
            //mockSubController.SetupGet(e => e.LoggingClient).Returns(mockLoggingClient.Object);
            mockSubController.Setup(e => e.GetDeviceCancellationBroker()).Returns(mockDeviceCancellationBroker.Object);

            subject = new DeviceSanityCheckSubStateAction(mockSubController.Object);

            asyncManager = new DeviceSubStateMachineAsyncManager(ref mockSubController, subject);

            using (kernel = new StandardKernel())
            {
                kernel.Bind<DeviceGetStatusSubStateAction>().ToSelf();
                kernel.Inject(subject);
            }
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void SubWorkflowStateType_Should_Equal_SanityCheck()
                    => Assert.Equal(DeviceSubWorkflowState.SanityCheck, subject.WorkflowStateType);

        [Fact]
        public async void DoWork_ShouldSimplyComplete_When_NoStateObjectProvided()
        {
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());
        }

        [Fact]
        public async void DoWork_ShouldDoWorkAndComplete_When_StateObjectIsProvided()
        {
            subject.SetState(linkRequest);

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //Assert.Null(linkRequest.LinkObjects.LinkActionResponseList[0].Errors);
        }

        [Fact]
        public async void DoWork_ShouldFailRequest_When_TimeoutPolicyReturnsFailure()
        {
            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = "DeviceMockerInc",
                Model = "DeviceMokerModel",
                SerialNumber = "CEEEDEADBEEF"

            };
            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
            fakeDeviceTwo.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<bool>(new Exception("Request timed out"));

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, bool>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            mockSubController.Setup(e => e.DidTimeoutOccur).Returns(true);

            linkRequest.Actions[0].Action = LinkAction.DALAction;
            subject.SetState(linkRequest);

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());
        }

        #region --- multiple devices ---
        [Fact]
        public async void DoWork_ShouldDoWorkAndCompleteWithoutError_WhenCalledWith_MultipleDevices()
        {
            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = "DeviceMockerInc",
                Model = "DeviceMokerModel",
                SerialNumber = "CEEEDEADBEEF"

            };

            var timeoutPolicy = PollyPolicyResultGenerator.GetSuccessfulPolicy<bool>();

            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
            fakeDeviceTwo.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, bool>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            mockSubController.Setup(e => e.DidTimeoutOccur).Returns(true);

            linkRequest.Actions[0].Action = LinkAction.Payment;
            subject.SetState(linkRequest);
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync("Unable to recover device.",
            //    It.IsAny<Dictionary<string, object>>()), Times.Never());

            mockSubController.Verify(e => e.Complete(subject), Times.Once());
        }

        [Fact]
        public async void DoWork_ShouldDoWorkAndCompleteWithoutError_WithSaveState_WhenCalledWith_MultipleDevices()
        {
            var timeoutPolicy = PollyPolicyResultGenerator.GetSuccessfulPolicy<bool>();

            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, bool>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            mockSubController.Setup(e => e.DidTimeoutOccur).Returns(true);
            subject.SetState(linkRequest);
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync("Unable to recover device.",
            //    It.IsAny<Dictionary<string, object>>()), Times.Never());

            mockSubController.Verify(e => e.SaveState(linkRequest), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());
            //Assert.Null(linkRequest.LinkObjects.LinkActionResponseList[0].Errors);
        }

        [Fact]
        public async void DoWork_ShouldDoWorkAndCompleteWithError_WhenCalledWith_MultipleDevices()
        {
            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = "DeviceMockerInc",
                Model = "DeviceMokerModel",
                SerialNumber = "CEEEDEADBEEF"
            };

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<bool>(new Exception("Request timed out"));

            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
            fakeDeviceTwo.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, bool>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            mockSubController.Setup(e => e.DidTimeoutOccur).Returns(true);

            linkRequest.Actions[0].Action = LinkAction.Payment;
            subject.SetState(linkRequest);
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync("Unable to recover device.",
            //    It.IsAny<Dictionary<string, object>>()), Times.Once());

            mockSubController.Verify(e => e.Complete(subject), Times.Once());
        }
        #endregion --- multiple devices ---
    }
}
