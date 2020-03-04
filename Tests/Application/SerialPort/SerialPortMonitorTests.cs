using DEVICE_CORE.SerialPort;
using DEVICE_CORE.SerialPort.Interfaces;
using System;
using System.Management;
using TestHelper;
using Xunit;

namespace DEVICE_CORE.Tests.SerialPort
{
    public class SerialPortMonitorTests : IDisposable
    {
        ISerialPortMonitor subject;

        public SerialPortMonitorTests()
        {
            subject = new SerialPortMonitor();
        }

        public void Dispose()
        {
            subject?.Dispose();
        }

        [Fact]
        public void StartMonitoring_ShouldSetArrivalAndRemoval_When_Called()
        {
            subject.StartMonitoring();

            var actualArrival = Helper.GetFieldValueFromInstance<ManagementEventWatcher>("arrival", false, false, subject);
            var actualRemoval = Helper.GetFieldValueFromInstance<ManagementEventWatcher>("removal", false, false, subject);

            Assert.NotNull(actualArrival);
            Assert.NotNull(actualRemoval);
        }

        [Fact]
        public void StopMonitoring_ShouldSetArrivalAndRemoval_When_Called()
        {
            subject.StopMonitoring();

            var actualArrival = Helper.GetFieldValueFromInstance<ManagementEventWatcher>("arrival", false, false, subject);
            var actualRemoval = Helper.GetFieldValueFromInstance<ManagementEventWatcher>("removal", false, false, subject);

            Assert.Null(actualArrival);
            Assert.Null(actualRemoval);
        }
    }
}
