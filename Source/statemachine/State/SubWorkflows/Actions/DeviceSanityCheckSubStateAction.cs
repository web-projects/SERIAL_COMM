﻿using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using LinkRequestExtensions;
using StateMachine.Cancellation;
using StateMachine.Helpers;
using StateMachine.State.Enums;
using System;
using System.Threading.Tasks;
using XO.Device;
using XO.Requests;
using static StateMachine.State.Enums.DeviceSubWorkflowState;

namespace StateMachine.State.SubWorkflows.Actions
{
    internal class DeviceSanityCheckSubStateAction : DeviceBaseSubStateAction
    {
        public override DeviceSubWorkflowState WorkflowStateType => SanityCheck;

        public DeviceSanityCheckSubStateAction(IDeviceSubStateController _) : base(_) { }

        public async override Task DoWork()
        {
            if (Controller.DidTimeoutOccur || Controller.DeviceEvent != DeviceEvent.None)
            {
                // recover device to idle
                IDeviceCancellationBroker cancellationBroker = Controller.GetDeviceCancellationBroker();

                LinkRequest linkRequest = StateObject as LinkRequest;
                LinkDeviceIdentifier deviceIdentifier = linkRequest.GetDeviceIdentifier();

                ICardDevice cardDevice = FindTargetDevice(deviceIdentifier);
                if (cardDevice != null)
                {
                    var timeoutPolicy = await cancellationBroker.ExecuteWithTimeoutAsync<bool>(
                                        _ => cardDevice.DeviceRecovery(),
                                        DeviceConstants.DeviceRecoveryTimeout,
                                        this.CancellationToken);

                    if (timeoutPolicy.Outcome == Polly.OutcomeType.Failure)
                    {
                        //_ = Controller.LoggingClient.LogErrorAsync("Unable to recover device.");
                        Console.WriteLine("Unable to recover device.");
                    }
                }
            }

            if (StateObject != null)
            {
                Controller.SaveState(StateObject);
            }

            _ = Complete(this);
        }
    }
}
