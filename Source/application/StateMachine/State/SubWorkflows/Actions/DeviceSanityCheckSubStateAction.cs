using DEVICE_CORE.Helpers;
using DEVICE_CORE.StateMachine.Cancellation;
using DEVICE_CORE.StateMachine.State.Enums;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using LinkRequestExtensions;
using System;
using System.Threading.Tasks;
using XO.Device;
using XO.Requests;
using static DEVICE_CORE.StateMachine.State.Enums.DeviceSubWorkflowState;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows.Actions
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
