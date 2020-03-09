using Devices.Common;
using Devices.Verifone;
using Devices.Verifone.Connection;
using Devices.Verifone.VIPA;
using Moq;
using Ninject;
using XO.Requests;
using Xunit;

namespace Devices.Simulator.Tests
{
    public class VerifoneDeviceTests
    {
        readonly VerifoneDevice subject;

        readonly DeviceInformation deviceInformation;
        readonly SerialDeviceConfig serialConfig;

        Mock<IVIPADevice> moqIVAPADevice;

        public VerifoneDeviceTests()
        {
            moqIVAPADevice = new Mock<IVIPADevice>();

            serialConfig = new SerialDeviceConfig
            {
                CommPortName = "COM9"
            };

            deviceInformation = new DeviceInformation()
            {
                ComPort = "COM9",
                Manufacturer = "Simulator",
                Model = "SimCity",
                SerialNumber = "CEEEDEADBEEF",
                ProductIdentification = "SIMULATOR",
                VendorIdentifier = "BADDCACA"
            };

            subject = new VerifoneDevice();

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Settings.InjectNonPublic = true;
                kernel.Settings.InjectParentPrivateProperties = true;

                kernel.Bind<IVIPADevice>().ToConstant(moqIVAPADevice.Object).WithConstructorArgument(deviceInformation);
                kernel.Inject(subject);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Probe_ReturnsProperActiveState_WhenCalled(bool expectedValue)
        {
            moqIVAPADevice.Setup(e => e.Connect(It.IsAny<string>(), It.IsAny<SerialConnection>())).Returns(expectedValue);

            DeviceConfig deviceConfig = new DeviceConfig()
            {
                Valid = true
            };

            subject.Probe(deviceConfig, deviceInformation, out bool actualValue);
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void GetStatus_ExpectedValue_WhenCalled()
        {
            LinkRequest expectedValue = new LinkRequest();
            var actualValue = subject.GetStatus(expectedValue);
            Assert.Equal(expectedValue, actualValue);
        }
    }
}
