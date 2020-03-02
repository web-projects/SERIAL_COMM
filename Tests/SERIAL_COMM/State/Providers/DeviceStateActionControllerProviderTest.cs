using Moq;
using SERIAL_COMM.StateMachine.State.Actions.Controllers;
using SERIAL_COMM.StateMachine.State.Management;
using SERIAL_COMM.StateMachine.State.Providers;
using System;
using Xunit;

namespace SERIAL_COMM.Tests.State.Providers
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
