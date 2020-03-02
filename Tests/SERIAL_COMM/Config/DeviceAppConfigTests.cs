using SERIAL_COMM.Config;
using SERIAL_COMM.Tests.Helpers;
using System;
using Xunit;

namespace SERIAL_COMM.Tests.Config
{
    public class DeviceAppConfigTests
    {
        readonly DeviceAppConfig subject;

        public DeviceAppConfigTests()
        {
            subject = new DeviceAppConfig();
        }

        [Fact]
        public void SetServicerType_When_ServicerTypeProvided()
        {
            const string expectedValue = "Mock";
            string actualValue = subject.SetDeviceProvider(expectedValue).DeviceProvider.ToString();
            Assert.Equal(expectedValue, actualValue);
        }

        [Fact]
        public void SetServicerType_ThrowWhen_UnknownType()
        {
            string randomString = RandomGenerator.BuildRandomString(6);
            Assert.Throws<ArgumentException>(() => subject.SetDeviceProvider(randomString).DeviceProvider);
        }
    }
}
