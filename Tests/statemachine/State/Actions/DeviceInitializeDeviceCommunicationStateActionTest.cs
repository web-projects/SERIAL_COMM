using Config;
using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using StateMachine.Tests;
using DEVICE_SDK.Sdk;
using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XO.Device;
using XO.Requests;
using Xunit;
using TestHelper.Polly;
using XO.Responses;
using StateMachine.Cancellation;
using System.Threading;

namespace StateMachine.State.Actions.Tests
{
    public class DeviceInitializeDeviceCommunicationStateActionTest : IDisposable
    {
        readonly DeviceInitializeDeviceCommunicationStateAction subject;

        readonly LinkRequest linkRequest;

        List<ICardDevice> moqCardDevices = new List<ICardDevice>();
        readonly Mock<ICardDevice> fakeDeviceOne = new Mock<ICardDevice>();
        readonly Mock<ICardDevice> fakeDeviceTwo = new Mock<ICardDevice>();
        readonly DeviceInformation deviceInformation;

        //readonly Mock<ILoggingServiceClient> mockLoggingClient;
        readonly Mock<IDeviceStateController> mockController;
        readonly Mock<IDevicePluginLoader> mockDevicePluginLoader;

        readonly Mock<IDeviceCancellationBroker> mockCancellationBroker;

        readonly DeviceStateMachineAsyncManager asyncManager;

        readonly string pluginPath = "some_plugin_path";
        readonly DeviceSection deviceSection;

        public DeviceInitializeDeviceCommunicationStateActionTest()
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

            deviceSection = new DeviceSection();

            //mockLoggingClient = new Mock<ILoggingServiceClient>();
            mockDevicePluginLoader = new Mock<IDevicePluginLoader>();
            
            mockCancellationBroker = new Mock<IDeviceCancellationBroker>();

            mockController = new Mock<IDeviceStateController>();
            //mockController.SetupGet(e => e.LoggingClient).Returns(mockLoggingClient.Object);
            mockController.SetupGet(e => e.DevicePluginLoader).Returns(mockDevicePluginLoader.Object);
            mockController.SetupGet(e => e.Configuration).Returns(deviceSection);
            mockController.SetupGet(e => e.PluginPath).Returns(pluginPath);
            mockController.Setup(e => e.GetCancellationBroker()).Returns(mockCancellationBroker.Object);

            deviceInformation = new DeviceInformation()
            {
                Manufacturer = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Manufacturer,
                Model = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.Model,
                SerialNumber = linkRequest.Actions[0].DeviceRequest.DeviceIdentifier.SerialNumber,
            };
            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(deviceInformation);
            moqCardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });
            mockController.SetupGet(e => e.TargetDevices).Returns(moqCardDevices);

            subject = new DeviceInitializeDeviceCommunicationStateAction(mockController.Object);

            asyncManager = new DeviceStateMachineAsyncManager(ref mockController, subject);
        }

        public void Dispose() => asyncManager.Dispose();

        [Fact]
        public void WorkflowStateType_Should_Equal_InitializeDeviceCommunication()
            => Assert.Equal(DeviceWorkflowState.InitializeDeviceCommunication, subject.WorkflowStateType);

        [Fact]
        public void DoWork_ShouldInstantiateNoDevice_WhenNoDeviceIsFound()
        {
            bool dalActive = true;
            //mockController.SetupGet(e => e.TargetDevice).Returns(It.IsAny<ICardDevice>);
            //mockCardDevice.Setup(e => e.Probe(It.IsAny<DeviceConfig>(), It.IsAny<DeviceInformation>(), out dalActive));

            List<ICardDevice> iCardDevices = new List<ICardDevice>();
            //var noDevice = new NoDevice.NoDevice();
            //noDevice.SortOrder = 1;
            //iCardDevices.Add(noDevice);
            //mockDevicePluginLoader.Setup(e => e.FindAvailableDevices(pluginPath)).Returns(iCardDevices);
            //mockController.SetupGet(e => e.TargetDevice).Returns(new NoDevice.NoDevice());

            Assert.Equal(subject.DoWork(), Task.CompletedTask);

            Assert.True(asyncManager.WaitFor());

            mockController.Verify(e => e.Complete(subject));
            //mockController.Verify(e => e.SetTargetDevice(It.IsAny<ICardDevice>()), Times.Once());
            //mockController.Verify(e => e.SetTargetDevice(It.IsAny<NoDevice.NoDevice>()), Times.Once());
        }

        [Fact]
        public void DoWork_ShouldComplete_When_NoDeviceIsSpecified()
        {
            //mockController.SetupGet(e => e.TargetDevice).Returns(new NoDevice.NoDevice());

            Assert.Equal(subject.DoWork(), Task.CompletedTask);
            Assert.True(asyncManager.WaitFor());

            mockController.Verify(e => e.Complete(subject));
            //mockLoggingClient.Verify(e => e.LogInfoAsync(It.IsAny<string>(), null), Times.Exactly(2));
        }

        [Fact]
        public void DoWork_ShouldProbeAllFoundDevices_When_Called()
        {
            bool dalActive = true;
            //mockCardDevice.Setup(e => e.Probe(It.IsAny<DeviceConfig>(), It.IsAny<DeviceInformation>(), out dalActive));

            List<ICardDevice> iCardDevices = new List<ICardDevice>()
            {
                new Devices.Simulator.DeviceSimulator()
            };
            mockDevicePluginLoader.Setup(e => e.FindAvailableDevices(pluginPath)).Returns(iCardDevices);

            Assert.Equal(subject.DoWork(), Task.CompletedTask);
            Assert.True(asyncManager.WaitFor());

            mockDevicePluginLoader.Verify(e => e.FindAvailableDevices(pluginPath), Times.Once());

            //mockController.Verify(e => e.SetTargetDevice(It.IsAny<ICardDevice>()), Times.Once());
            //mockController.Verify(e => e.SetTargetDevice(It.IsAny<NoDevice.NoDevice>()), Times.Once());
            //mockCardDevice.Verify(e => e.DeviceSetIdle(), Times.Once());

            //mockController.Verify(e => e.Configuration, Times.Once());
            mockController.Verify(e => e.Complete(subject));
        }

        [Fact]
        public async void DoWork_ShouldFailRequest_When_TimeoutPolicyReturnsFailure()
        {
            List<ICardDevice> iCardDevices = new List<ICardDevice>()
            {
                new Devices.Simulator.DeviceSimulator()
            };
            mockDevicePluginLoader.Setup(e => e.FindAvailableDevices(pluginPath)).Returns(iCardDevices);

            DeviceInformation deviceInformation = new DeviceInformation()
            {
                Manufacturer = "DeviceMocker",
                Model = "DeviceMock",
                SerialNumber = "CEEDEADBEEF"
            };
            fakeDeviceOne.Setup(e => e.DeviceInformation).Returns(deviceInformation);

            var timeoutPolicy = PollyPolicyResultGenerator.GetFailurePolicy<List<LinkErrorValue>>(new Exception("Request timed out"));

            mockCancellationBroker.Setup(e => e.ExecuteWithTimeoutAsync(It.IsAny<Func<CancellationToken, List<LinkErrorValue>>>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(timeoutPolicy));

            await subject.DoWork();

            Assert.True(asyncManager.WaitFor());

            //mockLoggingClient.Verify(e => e.LogErrorAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()), Times.Once());

            mockController.Verify(e => e.Error(subject), Times.Once());
            mockController.Verify(e => e.Complete(subject), Times.Once());
        }
    }
}
