using DEVICE_CORE.Helpers;
using DEVICE_CORE.StateMachine.Cancellation;
using DEVICE_CORE.StateMachine.State.Enums;
using Devices.Common.Interfaces;
using LinkRequestExtensions;
using System;
using System.Threading.Tasks;
using XO.Device;
using XO.Requests;
using static DEVICE_CORE.StateMachine.State.Enums.DeviceSubWorkflowState;

namespace DEVICE_CORE.State.SubWorkflows.Actions
{
    internal class DeviceResetCommandSubStateAction : DeviceBaseSubStateAction
    {
        public override DeviceSubWorkflowState WorkflowStateType => AbortCommand;

        public DeviceResetCommandSubStateAction(IDeviceSubStateController _) : base(_) { }

        public override SubStateActionLaunchRules LaunchRules => new SubStateActionLaunchRules
        {
            RequestCancellationToken = true
        };

        public override async Task DoWork()
        {
            if (StateObject is null)
            {
                //_ = Controller.LoggingClient.LogErrorAsync("Unable to find a state object while attempting to call reset command.");
                Console.WriteLine("Unable to find a state object while attempting to call reset command.");
                _ = Error(this);
            }
            else
            {
                LinkRequest linkRequest = StateObject as LinkRequest;
                LinkDeviceIdentifier deviceIdentifier = linkRequest.GetDeviceIdentifier();
                IDeviceCancellationBroker cancellationBroker = Controller.GetDeviceCancellationBroker();

                ICardDevice cardDevice = FindTargetDevice(deviceIdentifier);
                if (cardDevice != null)
                {
                    var timeoutPolicy = await cancellationBroker.ExecuteWithTimeoutAsync<LinkRequest>(
                    _ => cardDevice.AbortCommand(linkRequest),
                    DeviceConstants.CardCaptureTimeout,
                    this.CancellationToken);

                    if (timeoutPolicy.Outcome == Polly.OutcomeType.Failure)
                    {
                        //_ = Controller.LoggingClient.LogErrorAsync($"Unable to process Reset request from device - '{Controller.DeviceEvent}'.");
                        Console.WriteLine($"Unable to process Reset request from device - '{Controller.DeviceEvent}'.");
                        BuildSubworkflowErrorResponse(linkRequest, cardDevice.DeviceInformation, Controller.DeviceEvent);
                    }
                }
                else
                {
                    UpdateRequestDeviceNotFound(linkRequest, deviceIdentifier);
                }

                Controller.SaveState(linkRequest);

                _ = Complete(this);
            }
        }
    }
}
