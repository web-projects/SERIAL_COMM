using Devices.Common;
using Devices.Simulator.Connection;
using Moq;
using Ninject;
using XO.Requests;
using Xunit;

namespace Devices.Simulator.Tests
{
    public class DeviceSimulatorTests
    {
        readonly DeviceSimulator subject;

        Mock<ISerialConnection> moqSerialConnection;

        public DeviceSimulatorTests()
        {
            subject = new DeviceSimulator();

            moqSerialConnection = new Mock<ISerialConnection>();

            using (IKernel kernel = new StandardKernel())
            {
                kernel.Rebind<ISerialConnection>().ToConstant(moqSerialConnection.Object);
                kernel.Inject(subject);
            }
        }

        [Fact]
        public void Probe_ReturnsActiveTrue_WhenCalled()
        {
            DeviceConfig deviceConfig = new DeviceConfig()
            {
                Valid = true
            };
            SerialDeviceConfig serialConfig = new SerialDeviceConfig
            {
                CommPortName = "COM9"
            };
            deviceConfig.SetSerialDeviceConfig(serialConfig);

            DeviceInformation deviceInformation = new DeviceInformation()
            {
                ComPort = "COM9",
                Manufacturer = "Simulator",
                Model = "SimCity",
                SerialNumber = "CEEEDEADBEEF",
                ProductIdentification = "SIMULATOR",
                VendorIdentifier = "BADDCACA"
            };

            moqSerialConnection.Setup(e => e.Connect(false)).Returns(true);

            subject.Probe(deviceConfig, deviceInformation, out bool active);
            Assert.True(active);
        }

        [Fact]
        public void GetStatus_ThrowsNotImplemented_WhenCalled()
        {
            LinkRequest objRequest = new LinkRequest();
            Assert.Throws<System.NotImplementedException>(() => subject.GetStatus(objRequest));
        }
    }
}
