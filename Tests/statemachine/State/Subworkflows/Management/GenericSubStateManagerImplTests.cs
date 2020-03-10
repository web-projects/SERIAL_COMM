using DEVICE_CORE.Helpers.Tests;
using StateMachine.State.Actions.SubWorkflows.Tests;
using StateMachine.State.Interfaces;
using StateMachine.State.SubWorkflows;
using StateMachine.State.SubWorkflows.Actions;
using StateMachine.State.SubWorkflows.Management;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq;
using XO.Requests;

namespace IPA5.Devices.DAL.Core.Tests.State.Actions.SubWorkflows.Management
{
    public class GenericSubStateManagerImplTests
    {
        readonly GenericSubStateManagerImpl subject;

        readonly Mock<IDeviceStateController> mockIDeviceStateController;
        readonly Mock<IDeviceSubStateController> mockIDeviceSubStateController;

        readonly DeviceSubStateMachineAsyncManager asyncManager;

        IDeviceSubStateAction stateIDeviceSubStateAction;

        public GenericSubStateManagerImplTests()
        {
            mockIDeviceSubStateController = new Mock<IDeviceSubStateController>();
            mockIDeviceStateController = new Mock<IDeviceStateController>();

            subject = new GenericSubStateManagerImpl(mockIDeviceStateController.Object);

            stateIDeviceSubStateAction = new DeviceRequestCompleteSubStateAction(mockIDeviceSubStateController.Object);
            asyncManager = new DeviceSubStateMachineAsyncManager(ref mockIDeviceSubStateController, stateIDeviceSubStateAction);
        }

        [Fact]
        public void LaunchWorkflow_ShouldInvokeCallToGetNextAction_When_Called()
        {
            WorkflowOptions launchOptions = new WorkflowOptions()
            {
                ExecutionTimeout = 1000,
                StateObject = RequestBuilder.LinkRequestGetDeviceStatus()
            };

            subject.LaunchWorkflow(launchOptions);

            var stack = TestHelper.Helper.GetFieldValueFromInstance<Stack<object>>("savedStackState", false, false, subject);

            Assert.Empty(stack);

            subject.Dispose();
        }

        [Fact]
        public void LaunchWorkflow_ShouldComplete_When_Called()
        {
            Assert.Equal(subject.Complete(stateIDeviceSubStateAction), Task.CompletedTask);

            var stack = TestHelper.Helper.GetFieldValueFromInstance<Stack<object>>("savedStackState", false, false, subject);

            Assert.Empty(stack);

            subject.Dispose();
        }

        [Fact]
        public void LaunchWorkflow_ShouldFireGlobalTimer_When_Timeout()
        {
            LinkRequest linkRequest = RequestBuilder.LinkRequestGetDeviceStatus();

            WorkflowOptions launchOptions = new WorkflowOptions()
            {
                ExecutionTimeout = linkRequest.Timeout,
                StateObject = linkRequest
            };

            subject.LaunchWorkflow(launchOptions);

            // linkRequest.Timeout should be less then 2000 ms.
            asyncManager.WaitFor();

            Assert.Equal(subject.Complete(stateIDeviceSubStateAction), Task.CompletedTask);
            Assert.True(subject.DidTimeoutOccur);

            subject.Dispose();
        }
    }
}
