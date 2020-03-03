﻿using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System.Threading.Tasks;

namespace DEVICE_CORE.StateMachine.State.Actions
{
    internal class DeviceManageStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.Manage;

        public DeviceManageStateAction(IDeviceStateController _) : base(_) { }

        public override bool DoDeviceDiscovery()
        {
            LastException = new StateException("device recovery is needed");
            _ = Error(this);
            return true;
        }

        public override Task DoWork()
        {
            return Task.CompletedTask;
        }

        //public override void RequestReceived(LinkRequest request)
        public override void RequestReceived(object request)
        {
            Controller.SaveState(request);

            _ = Complete(this);
        }
    }
}