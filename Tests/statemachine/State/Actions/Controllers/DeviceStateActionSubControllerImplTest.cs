using StateMachine.State.Enums;
using StateMachine.State.SubWorkflows;
using StateMachine.State.SubWorkflows.Actions;
using StateMachine.State.SubWorkflows.Actions.Controllers;
using StateMachine.State.TestStubs.Tests;
using System;
using System.Collections.Generic;
using Xunit;

namespace StateMachine.State.Actions.Controllers.Tests
{
    public class DeviceStateActionSubControllerImplTest
    {
        readonly static StubDeviceSubStateManager stubManager = new StubDeviceSubStateManager();
        readonly static DeviceStateActionSubControllerImpl subject = new DeviceStateActionSubControllerImpl(stubManager);

        [Theory]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), DeviceSubWorkflowState.GetStatus)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), DeviceSubWorkflowState.GetStatus, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), DeviceSubWorkflowState.AbortCommand)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), DeviceSubWorkflowState.AbortCommand, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), DeviceSubWorkflowState.ResetCommand)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), DeviceSubWorkflowState.ResetCommand, true, true)]
        [InlineData(typeof(DeviceRequestCompleteSubStateAction), DeviceSubWorkflowState.SanityCheck)]
        [InlineData(typeof(DeviceRequestCompleteSubStateAction), DeviceSubWorkflowState.SanityCheck, true, true)]
        public void GetNextAction_ShouldReturnCorrectType_When_Called(Type expectedType, DeviceSubWorkflowState initialState, bool set = true, bool exception = false)
        {
            TestHelper.Helper.SetFieldValueToInstance<IDeviceStateAction>("currentStateAction", false, false, subject, null);

            if (set)
            {
                var map = TestHelper.Helper.GetFieldValueFromInstance<Dictionary<DeviceSubWorkflowState, Func<IDeviceSubStateController, IDeviceSubStateAction>>>(
                    "workflowMap", false, false, subject);

                IDeviceSubStateAction action = map[initialState](stubManager);

                if (exception)
                {
                    TestHelper.Helper.SetPropertyValueToInstance<StateException>("LastException", true, false, action, new StateException());
                }

                TestHelper.Helper.SetFieldValueToInstance<IDeviceStateAction>("currentStateAction", false, false, subject, action);
            }

            Assert.IsType(expectedType, subject.GetNextAction(initialState));
        }
    }
}
