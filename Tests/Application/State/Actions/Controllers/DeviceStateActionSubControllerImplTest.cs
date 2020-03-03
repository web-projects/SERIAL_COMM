using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace DEVICE_CORE.Tests.State.Actions.Controllers
{
    public class DeviceStateActionSubControllerImplTest
    {
        /*readonly static StubDeviceSubStateManager stubManager = new StubDeviceSubStateManager();
        readonly static DeviceStateActionSubControllerImpl subject = new DeviceStateActionSubControllerImpl(stubManager);

        [Theory]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetStatus)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetStatus, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), PresentCard)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), PresentCard, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetCardData)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetCardData, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetCreditOrDebit)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetCreditOrDebit, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetPin)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetPin, true, true)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetZip)]
        [InlineData(typeof(DeviceSanityCheckSubStateAction), GetZip, true, true)]
        [InlineData(typeof(DeviceRequestCompleteSubStateAction), SanityCheck)]
        [InlineData(typeof(DeviceRequestCompleteSubStateAction), SanityCheck, true, true)]
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
        }*/
    }
}
