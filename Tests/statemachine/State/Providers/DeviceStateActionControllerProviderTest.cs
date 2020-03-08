using DEVICE_CORE.StateMachine.State.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.Management;
using Moq;
using System;
using Xunit;

namespace DEVICE_CORE.StateMachine.State.Providers.Tests
{
    public class DeviceStateActionControllerProviderTest
    {
        readonly DeviceStateActionControllerProvider subject;
        readonly Mock<IDeviceStateManager> mockStateManager;

        public DeviceStateActionControllerProviderTest()
        {
            mockStateManager = new Mock<IDeviceStateManager>();

            subject = new DeviceStateActionControllerProvider();
        }

        [Fact]
        public void GetStateActionController_ShouldThrowArgumentNullException_When_ManagerIsNull()
            => Assert.Throws<ArgumentNullException>(() => subject.GetStateActionController(null));

        [Fact]
        public void GetStateActionController_ShouldReturnActionController_When_ManagerIsProvided()
        {
            IDeviceStateActionController stateActionController = subject.GetStateActionController(mockStateManager.Object);

            Assert.IsType<DeviceStateActionControllerImpl>(stateActionController);
        }
    }
}
