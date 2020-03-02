using SERIAL_COMM.Providers;
using SERIAL_COMM.StateMachine.Cancellation;
using Xunit;

namespace SERIAL_COMM.Tests.State.Providers
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
