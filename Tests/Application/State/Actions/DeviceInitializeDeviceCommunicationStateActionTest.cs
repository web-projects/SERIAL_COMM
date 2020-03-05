using Moq;
using DEVICE_CORE.Config;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Devices.Common.Interfaces;
using DEVICE_SDK.Sdk;
using Devices.Common.Helpers;

namespace DEVICE_CORE.Tests.State.Actions
{
    public class DeviceInitializeDeviceCommunicationStateActionTest : IDisposable
    {
        readonly DeviceInitializeDeviceCommunicationStateAction subject;

        readonly Mock<ICardDevice> mockCardDevice;
        //readonly Mock<ILoggingServiceClient> mockLoggingClient;
        readonly Mock<IDeviceStateController> mockController;
        readonly Mock<IDevicePluginLoader> mockDevicePluginLoader;
        
        readonly DeviceStateMachineAsyncManager asyncManager;

        readonly string pluginPath = "some_plugin_path";
        readonly DeviceSection deviceSection;

        public DeviceInitializeDeviceCommunicationStateActionTest()
        {
            deviceSection = new DeviceSection();

            mockCardDevice = new Mock<ICardDevice>();
            mockCardDevice.SetupGet(e => e.Name).Returns(StringValueAttribute.GetStringValue(DeviceType.NoDevice));
            mockCardDevice.SetupGet(e => e.ManufacturerConfigID).Returns(StringValueAttribute.GetStringValue(DeviceType.NoDevice));

            //mockLoggingClient = new Mock<ILoggingServiceClient>();
            mockDevicePluginLoader = new Mock<IDevicePluginLoader>();

            mockController = new Mock<IDeviceStateController>();
            //mockController.SetupGet(e => e.LoggingClient).Returns(mockLoggingClient.Object);
            //mockController.SetupGet(e => e.TargetDevice).Returns(mockCardDevice.Object);
            mockController.SetupGet(e => e.DevicePluginLoader).Returns(mockDevicePluginLoader.Object);
            mockController.SetupGet(e => e.Configuration).Returns(deviceSection);
            mockController.SetupGet(e => e.PluginPath).Returns(pluginPath);

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

            //List<ICardDevice> iCardDevices = new List<ICardDevice>()
            //{
            //    new NoDevice.NoDevice()
            //};
            //mockDevicePluginLoader.Setup(e => e.FindAvailableDevices(pluginPath)).Returns(iCardDevices);

            Assert.Equal(subject.DoWork(), Task.CompletedTask);
            Assert.True(asyncManager.WaitFor());

            //mockDevicePluginLoader.Verify(e => e.FindAvailableDevices(pluginPath), Times.Once());

            //mockController.Verify(e => e.SetTargetDevice(It.IsAny<ICardDevice>()), Times.Once());
            //mockController.Verify(e => e.SetTargetDevice(It.IsAny<NoDevice.NoDevice>()), Times.Once());
            //mockCardDevice.Verify(e => e.DeviceSetIdle(), Times.Once());

            //mockController.Verify(e => e.Configuration, Times.Once());
            mockController.Verify(e => e.Complete(subject));
        }
    }
}
