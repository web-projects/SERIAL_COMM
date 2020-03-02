using SERIAL_COMM.StateMachine.Cancellation;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace SERIAL_COMM.Tests.Cancellation
{
    public class DeviceCancellationBrokerImplTests
    {
        readonly DeviceCancellationBrokerImpl subject;

        public DeviceCancellationBrokerImplTests()
        {
            subject = new DeviceCancellationBrokerImpl();
        }

        [Theory]
        [InlineData(1, 2068)]
        [InlineData(2, 5120)]
        [InlineData(3, 1024)]
        [InlineData(4, 2048)]
        public async void ExecuteWithTimeoutAsync_ShouldMatchPolicy_When_CalledWithTimeout(int timeout, int delayMS)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            var result = await subject.ExecuteWithTimeoutAsync(new Func<CancellationToken, object>((CancellationToken token) =>
            {
                Thread.Sleep(delayMS);
                return null;
            }), timeout, CancellationToken.None);

            sw.Stop();

            if ((timeout * 1024) > delayMS)
            {
                Assert.True(sw.ElapsedMilliseconds < (timeout * 1024));
                Assert.Equal(Polly.OutcomeType.Successful, result.Outcome);
            }
            else
            {
                Assert.InRange(sw.ElapsedMilliseconds, (timeout * 1000), delayMS + 512);
                Assert.Equal(Polly.OutcomeType.Failure, result.Outcome);
            }
        }
    }
}
