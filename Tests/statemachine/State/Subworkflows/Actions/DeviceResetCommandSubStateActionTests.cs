using StateMachine.Cancellation;
using StateMachine.State.Actions.SubWorkflows.Tests;
using StateMachine.State.Enums;
using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using Moq;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestHelper.Polly;
using XO.Device;
using XO.Requests;
using Xunit;

namespace StateMachine.State.SubWorkflows.Actions.Tests
{
    public class DeviceResetCommandSubStateActionTests : IDisposable
    {
        readonly DeviceResetCommandSubStateAction subject;

        readonly Mock<IDeviceSubStateController> mockSubController;
        //readonly Mock<ILoggingServiceClient> mockLoggingClient;
        readonly Mock<IDeviceCancellationBroker> mockDeviceCancellationBroker;

        List<ICardDevice> cardDevices = new List<ICardDevice>();
        readonly Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
        readonly Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
        readonly DeviceInformation deviceInformation;

        readonly LinkRequest linkRequest;

        readonly DeviceSubStateMachineAsyncManager asyncManager;

        public DeviceResetCommandSubStateActionTests()
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
            //        Manufacturer = DeviceType.NoDevice.ToString(),
            //        CardWorkflowControls = new LinkCardWorkflowControls()
            //        {
            //            CardCaptureTimeout = 10,
            //            ManualCardTimeout = 5
            //        }
            //    }
            //};

            deviceInformation = new DeviceInformation()
            {
                Manufacturer = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Manufacturer,
                Model = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Model,
                SerialNumber = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.SerialNumber,
            };

            //mockLoggingClient = new Mock<ILoggingServiceClient>();

            mockDeviceCancellationBroker = new Mock<IDeviceCancellationBroker>();

            mockSubController = new Mock<IDeviceSubStateController>();
            //mockSubController.SetupGet(e => e.LoggingClient).Returns(mockLoggingClient.Object);
            mockSubController.Setup(e => e.GetDeviceCancellationBroker()).Returns(mockDeviceCancellationBroker.Object);

            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });
            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            subject = new DeviceResetCommandSubStateAction(mockSubController.Object);
            subject.SetState(linkRequest);

            asyncManager = new DeviceSubStateMachineAsyncManager(ref mockSubController, subject);

            using IKernel kernel = new StandardKernel();
            kernel.Bind<DeviceResetCommandSubStateAction>().ToSelf();
            kernel.Inject(subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void SubWorkflowStateType_Should_Equal_GetCardData()
                    => Assert.Equal(DeviceSubWorkflowState.ResetCommand, subject.WorkflowStateType);

        [Fact]
        public async void DoWork_ShouldSimplyError_When_NoStateObjectProvided()
        {
            subject.SetState(null);
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
            mockSubController.Verify(e => e.Error(subject), Times.Once());
        }

        [Fact]
        public async void DoWork_ShouldDoWorkAndComplete_When_StateObjectIsProvided()
        {
            var timeoutPolicy = PollyPolicyResultGenerator.GetSuccessfulPolicy<LinkRequest>();

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            mockSubController.Verify(e => e.SaveState(linkRequest), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());
        }

        [Fact]
        public async void DoWork_ShouldFailRequest_When_TimeoutPolicyReturnsFailure()
        {
            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = "DeviceMocker",
                Model = "DeviceMock",
                SerialNumber = "CEEDEADBEEF"
            };

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<LinkRequest>(new Exception("Request timed out"));

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());

            //Assert.NotNull(linkRequest.Actions[0].DeviceRequest.LinkObjects);
            //Assert.Equal(Enum.GetName(typeof(EventCodeType), EventCodeType.REQUEST_TIMEOUT), linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Code);

            mockSubController.Verify(e => e.SaveState(linkRequest), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());

            //DeviceInformation actualValue = new DeviceInformation()
            //{
            //    Manufacturer = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].Manufacturer,
            //    Model = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].Model,
            //    SerialNumber = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].SerialNumber
            //};
            //Assert.NotEqual(deviceInformation, actualValue);
        }

        [Fact]
        public async void DoWork_ShouldFailRequest_When_RequestTimeoutIsTriggered()
        {
            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<LinkRequest>(new Exception("Request timed out"));

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());

            //Assert.NotNull(linkRequest.Actions[0].DeviceRequest.LinkObjects);
            //Assert.Equal(Enum.GetName(typeof(EventCodeType), EventCodeType.REQUEST_TIMEOUT),
            //    linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Code);

            mockSubController.Verify(e => e.SaveState(linkRequest), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());

            //DeviceInformation actualValue = new DeviceInformation()
            //{
            //    Manufacturer = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].Manufacturer,
            //    Model = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].Model,
            //    SerialNumber = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].SerialNumber
            //};
            //Assert.NotEqual(deviceInformation, actualValue);
        }

        [Fact]
        public void DeviceEventReceived_ShouldLogEventAndDeviceSerialNumber_WhenCalled()
        {
            subject.DeviceEventReceived(DeviceEvent.CancelKeyPressed, deviceInformation);

            //mockLoggingClient.Verify(e => e.LogInfoAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
        }

        [Fact]
        public void ComEventReceived_ShouldLogEventAndPortNumber_WhenCalledWithRemovalEvent()
        {
            subject.ComportEventReceived(PortEventType.Removal, "COM9");

            //mockLoggingClient.Verify(e => e.LogInfoAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());
        }

        [Fact]
        public void ComEventReceived_ShouldNotLogEventAndPortNumber_WhenCalledWithInsertionEvent()
        {
            subject.ComportEventReceived(PortEventType.Insertion, "COM9");

            //mockLoggingClient.Verify(e => e.LogInfoAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Never());
        }

        [Theory]
        [InlineData("USER_CANCELED", "CancelKeyPressed", DeviceEvent.CancelKeyPressed)]
        [InlineData("CANCELLATION_REQUEST", "CancellationRequest", DeviceEvent.CancellationRequest)]
        [InlineData("DEVICE_UNPLUGGED", "DeviceUnplugged", DeviceEvent.DeviceUnplugged)]
        [InlineData("REQUEST_TIMEOUT", "RequestTimeout", DeviceEvent.RequestTimeout)]
        public async void DoWork_ShouldForceCompletionWithCancelation__WhenTestingGetCardData(string exception, string userEvent, DeviceEvent deviceEvent)
        {
            LinkActionRequest linkActionRequest = linkRequest.Actions.First();
            linkActionRequest.Action = LinkAction.Payment;
            linkActionRequest.DeviceRequest = new LinkDeviceRequest()
            {
                //LinkObjects = new LinkDeviceRequestIPA5Object()
                //{
                //    DeviceResponseData = new LinkDeviceActionResponse
                //    {
                //        Errors = new List<LinkErrorValue>
                //        {
                //            new LinkErrorValue
                //            {
                //                Code = exception,
                //                Description = "Canceled"
                //            }
                //        }
                //    }
                //}
            };
            string expectedValue = linkActionRequest.Action.ToString();

            Mock<ICardDevice> fakeDeviceThree = new Mock<ICardDevice>();
            fakeDeviceThree.Setup(e => e.ResetDevice(It.IsAny<LinkRequest>())).Returns(linkRequest);

            subject.SetState(linkRequest);

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<LinkRequest>(new Exception(exception));

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));
            mockSubController.Setup(e => e.DeviceEvent).Returns(deviceEvent);

            await subject.DoWork();

            //mockLoggingClient.Verify(e => e.LogErrorAsync(string.Format("Unable to obtain Card Data from device - '{0}'.", userEvent),
            //    It.IsAny<Dictionary<string, object>>()), Times.Once());

            //Assert.NotNull(linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors);
            //Assert.Equal(exception, linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Code);
            //Assert.Equal(expectedValue, linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Type);

            //DeviceInformation actualValue = new DeviceInformation()
            //{
            //    Manufacturer = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].Manufacturer,
            //    Model = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].Model,
            //    SerialNumber = linkRequest.LinkObjects.LinkActionResponseList[0].DeviceResponse.Devices[0].SerialNumber
            //};
            //Assert.NotEqual(deviceInformation, actualValue);
        }

        #region --- multiple devices 
        [Fact]
        public async void DoWork_ShouldFailWhenDeviceIdentifierIsNull_WhenCalled()
        {
            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();

            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(null as DeviceInformation);
            fakeDeviceTwo.Setup(e => e.DeviceInformation).Returns(null as DeviceInformation);

            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            linkRequest.Actions[0].DeviceRequest.DeviceIdentifier = null;
            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(string.Format($"Unable to obtain device information from request - '{DeviceDiscovery.NoDeviceSpecified}'."),
            //    It.IsAny<Dictionary<string, object>>()), Times.Once());
        }

        [Fact]
        public async void DoWork_ShouldFailRequest_When_TimeoutPolicyReturnsFailureForMultipleDevices()
        {
            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();

            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Manufacturer,
                Model = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Model,
                SerialNumber = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.SerialNumber
            };
            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            deviceInformation.SerialNumber = "DEADBEEFCEEE";
            fakeDeviceTwo.Setup(e => e.DeviceInformation).Returns(deviceInformation);

            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<LinkRequest>(new Exception("Request timed out"));

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());

            //Assert.NotNull(linkRequest.Actions[0].DeviceRequest.LinkObjects);
            //Assert.Equal(Enum.GetName(typeof(EventCodeType), EventCodeType.REQUEST_TIMEOUT), linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Code);

            mockSubController.Verify(e => e.SaveState(linkRequest), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());
        }

        [Fact]
        public async void DoWork_ShouldDoWorkAndComplete_When_StateObjectIsProvidedForMultipleDevices()
        {
            var timeoutPolicy = PollyPolicyResultGenerator.GetSuccessfulPolicy<LinkRequest>();

            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Manufacturer,
                Model = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Model,
                SerialNumber = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.SerialNumber,
            };

            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            mockSubController.Verify(e => e.SaveState(linkRequest), Times.Once());
            mockSubController.Verify(e => e.Complete(subject), Times.Once());
        }

        [Theory]
        [InlineData("USER_CANCELED", "CancelKeyPressed", DeviceEvent.CancelKeyPressed)]
        [InlineData("CANCELLATION_REQUEST", "CancellationRequest", DeviceEvent.CancellationRequest)]
        [InlineData("DEVICE_UNPLUGGED", "DeviceUnplugged", DeviceEvent.DeviceUnplugged)]
        [InlineData("REQUEST_TIMEOUT", "RequestTimeout", DeviceEvent.RequestTimeout)]
        public async void DoWork_ShouldForceCompletionWithCancelation__WhenTestingGetStatusForMultipleDevices(string exception, string userEvent, DeviceEvent deviceEvent)
        {
            List<ICardDevice> cardDevices = new List<ICardDevice>();
            Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
            Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();

            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Manufacturer,
                Model = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Model,
                SerialNumber = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.SerialNumber
            };

            LinkActionRequest linkActionRequest = linkRequest.Actions.First();
            linkActionRequest.Action = LinkAction.Payment;
            linkActionRequest.DeviceRequest = new LinkDeviceRequest()
            {
                //LinkObjects = new LinkDeviceRequestIPA5Object()
                //{
                //    DeviceResponseData = new LinkDeviceActionResponse
                //    {
                //        Errors = new List<LinkErrorValue>
                //        {
                //            new LinkErrorValue
                //            {
                //                Code = exception,
                //                Description = "Canceled"
                //            }
                //        }
                //    }
                //},
                DeviceIdentifier = new LinkDeviceIdentifier()
                {
                    Manufacturer = deviceInformation.Manufacturer,
                    Model = deviceInformation.Model,
                    SerialNumber = deviceInformation.SerialNumber
                }
            };
            string expectedValue = linkActionRequest.Action.ToString();

            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            fakeDeviceTwo.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            fakeDeviceOne.Setup(e => e.ResetDevice(It.IsAny<LinkRequest>())).Returns(linkRequest);
            fakeDeviceTwo.Setup(e => e.ResetDevice(It.IsAny<LinkRequest>())).Returns(linkRequest);

            cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            mockSubController.SetupGet(e => e.TargetDevices).Returns(cardDevices);

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<LinkRequest>(new Exception(exception));

            mockDeviceCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, LinkRequest>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));
            mockSubController.Setup(e => e.DeviceEvent).Returns(deviceEvent);

            await subject.DoWork();

            //mockLoggingClient.Verify(e => e.LogErrorAsync(string.Format("Unable to obtain Card Data from device - '{0}'.", userEvent),
            //    It.IsAny<Dictionary<string, object>>()), Times.Once());

            //Assert.NotNull(linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors);
            //Assert.Equal(exception, linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Code);
            //Assert.Equal(expectedValue, linkRequest.Actions[0].DeviceRequest.LinkObjects.DeviceResponseData.Errors[0].Type);
        }
        #endregion --- multiple devices ---
    }
}
