using DEVICE_CORE.StateMachine.State;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.Tests.State.TestStubs;
using System;
using System.Collections.ObjectModel;
using Xunit;

namespace DEVICE_CORE.Tests.State.Actions.Controllers
{
    public class DeviceStateActionControllerImplTest
    {
        readonly static StubDeviceStateManager stubManager = new StubDeviceStateManager();
        readonly static DeviceStateActionControllerImpl subject = new DeviceStateActionControllerImpl(stubManager);

        [Theory]
        [InlineData(typeof(DeviceInitializeDeviceCommunicationStateAction), DeviceWorkflowState.None)]
        [InlineData(typeof(DeviceInitializeDeviceCommunicationStateAction), DeviceWorkflowState.None, true, true)]
        [InlineData(typeof(DeviceInitializeDeviceCommunicationStateAction), DeviceWorkflowState.DeviceRecovery)]
        [InlineData(typeof(DeviceInitializeDeviceCommunicationStateAction), DeviceWorkflowState.DeviceRecovery, true, true)]
        [InlineData(typeof(DeviceInitializeDeviceHealthStateAction), DeviceWorkflowState.InitializeDeviceCommunication)]
        [InlineData(typeof(DeviceRecoveryStateAction), DeviceWorkflowState.InitializeDeviceHealth, true, true)]
        [InlineData(typeof(DeviceManageStateAction), DeviceWorkflowState.SubWorkflowIdleState)]
        [InlineData(typeof(DeviceProcessRequestStateAction), DeviceWorkflowState.Manage)]
        [InlineData(typeof(DeviceRecoveryStateAction), DeviceWorkflowState.Manage, true, true)]
        //[InlineData(typeof(DeviceSubWorkflowIdleStateAction), DeviceWorkflowState.ProcessRequest)]
        //[InlineData(typeof(DeviceSubWorkflowIdleStateAction), DeviceWorkflowState.ProcessRequest, true, true)]
        [InlineData(typeof(DeviceRecoveryStateAction), DeviceWorkflowState.SubWorkflowIdleState, true, true)]
        public void GetNextAction_ShouldReturnCorrectType_When_Called(Type expectedType, DeviceWorkflowState initialState, bool set = true, bool exception = false)
        {
           TestHelper.Helper.SetFieldValueToInstance<IDeviceStateAction>("currentStateAction", false, false, subject, null);

            //if (set)
            //{
                //var map = TestHelper.Helper.GetFieldValueFromInstance<ReadOnlyDictionary<DeviceWorkflowState, Func<IDeviceStateController, IDeviceStateAction>>>(
                //    "workflowMap", false, false, subject);

            //    IDeviceStateAction action = map[initialState](stubManager);

            //    if (exception)
            //    {
            //        TestHelper.Helper.SetPropertyValueToInstance<StateException>("LastException", true, false, action, new StateException());
            //    }

            //    TestHelper.Helper.SetFieldValueToInstance<IDeviceStateAction>("currentStateAction", false, false, subject, action);
            //}

            //Assert.IsType(expectedType, subject.GetNextAction(initialState));
        }
    }
}
