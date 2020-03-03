﻿using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System.Threading.Tasks;

namespace DEVICE_CORE.StateMachine.State.Actions
{
    internal class DeviceInitializeDeviceHealthStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.InitializeDeviceHealth;

        public DeviceInitializeDeviceHealthStateAction(IDeviceStateController _) : base(_) { }

        public override Task DoWork()
        {
            // TODO: Implement Device Health here.
            //Controller.LoggingClient?.LogInfoAsync($"Currently in the '{WorkflowStateType}' state with nothing to do.. skipping...");

            Controller.SetPublishEventHandlerAsTask();

            _ = Complete(this);

            return Task.CompletedTask;
        }
    }
}