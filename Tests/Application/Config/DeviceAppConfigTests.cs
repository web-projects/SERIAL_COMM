using DEVICE_CORE.Config;
using DEVICE_CORE.Tests.Helpers;
using System;
using Xunit;

namespace DEVICE_CORE.Tests.Config
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
