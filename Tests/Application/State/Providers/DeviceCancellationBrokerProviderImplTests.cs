﻿using DEVICE_CORE.Providers;
using DEVICE_CORE.StateMachine.Cancellation;
using Xunit;

namespace DEVICE_CORE.Tests.State.Providers
{
    public class DeviceCancellationBrokerProviderImplTests
    {
        readonly DeviceCancellationBrokerProviderImpl subject;

        public DeviceCancellationBrokerProviderImplTests()
        {
            subject = new DeviceCancellationBrokerProviderImpl();
        }

        [Fact]
        void ValidateProviderType_Matches_SubjectType()
           => Assert.IsType<DeviceCancellationBrokerImpl>(subject.GetDeviceCancellationBroker());
    }
}