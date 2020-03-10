using StateMachine.Providers;
using StateMachine.Cancellation;
using Xunit;

namespace StateMachine.State.Providers.Tests
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
