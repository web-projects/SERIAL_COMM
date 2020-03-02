﻿using SERIAL_COMM.Config;
using Xunit;

namespace SERIAL_COMM.Tests.Config
{
    public class DeviceConfigurationProviderTests
    {
        readonly IDeviceConfigurationProvider subject;

        public DeviceConfigurationProviderTests()
        {
            subject = new DeviceConfigurationProvider();
        }
        [Fact]
        public void GetAppConfig_ShouldReturnProperAppConfigType_When_Requested()
        {
            //IAppConfig actualObject = subject.GetAppConfig();
            //Assert.IsAssignableFrom<DeviceAppConfig>(actualObject);
        }

        [Fact]
        public void GetAppConfig_ShouldReturnValueWhichWasSet()
        {
            //string expectedValue = "Mock";
            //var config = subject.GetAppConfig();
            //config.SetDeviceProvider(expectedValue);
            //string actualValue = config.DeviceProvider.ToString();
            //Assert.Equal(expectedValue, actualValue);
        }
    }
}
