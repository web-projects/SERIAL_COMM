using Newtonsoft.Json;
using SERIAL_COMM.StateMachine.State.Enums;
using SERIAL_COMM.StateMachine.State.Interfaces;
using SERIAL_COMM.StateMachine.State.SubWorkflows;
using System;
using System.Threading.Tasks;

namespace SERIAL_COMM.StateMachine.State.Actions
{
    internal class DeviceProcessRequestStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.ProcessRequest;

        public DeviceProcessRequestStateAction(IDeviceStateController _) : base(_) { }

        public override async Task DoWork()
        {
            // TODO: Interpret what type of object is here so that we can handle it accordingly.
            /**
             * if (StateObject is LinkRequest)
             * if (StateObject is DeviceEvent)
             **/
            if (StateObject != null)
            {
                //await ProcessListenerRequest(StateObject as LinkRequest);
                await ProcessListenerRequest(StateObject as object);
            }

            _ = Complete(this);
        }

        //private async Task ProcessListenerRequest(LinkRequest linkRequest)
        private async Task ProcessListenerRequest(object linkRequest)
        {
            try
            {
                //_ = Controller.LoggingClient.LogInfoAsync($"Received from {linkRequest}");
                //LinkRequest linkRequestResponse = null;
                object linkRequestResponse = null;
                bool deviceConnected = false;

                // Setup workflow
                WorkflowOptions workflowOptions = new WorkflowOptions();
                workflowOptions.StateObject = linkRequest;

                Controller.SaveState(workflowOptions);

                // check for device connected
                /*if (Controller.TargetDevice == null && Controller.TargetDevices == null)
                {
                    WorkflowOptions workflowOptions = new WorkflowOptions();
                    workflowOptions.StateObject = linkRequest;

                    Controller.SaveState(workflowOptions);
                }
                else if (Controller.TargetDevice != null)
                {
                    deviceConnected = Controller.TargetDevice.IsConnected(linkRequest);
                    if (deviceConnected)
                    {
                        WorkflowOptions workflowOptions = new WorkflowOptions();
                        workflowOptions.StateObject = linkRequest;

                        Controller.SaveState(workflowOptions);
                    }
                }
                else
                {
                    WorkflowOptions workflowOptions = new WorkflowOptions();

                    // Payment: targeted device
                    if (linkRequest.Actions?[0]?.Action == LinkAction.Payment ||
                        linkRequest.Actions?[0]?.Action == LinkAction.DALAction)
                    {
                        LinkDeviceIdentifier deviceIdentifier = linkRequest.GetDeviceIdentifier();
                        ICardDevice cardDevice = FindTargetDevice(deviceIdentifier);
                        if (cardDevice != null)
                        {
                            deviceConnected = cardDevice.IsConnected(linkRequest);
                            if (deviceConnected)
                            {
                                workflowOptions.StateObject = linkRequest;
                            }
                        }
                    }
                    else
                    {
                        // Session: device discovery
                        foreach (var device in Controller.TargetDevices)
                        {
                            deviceConnected = device.IsConnected(linkRequest);
                            if (deviceConnected)
                            {
                                workflowOptions.StateObject = linkRequest;
                            }
                        }
                    }
                    Controller.SaveState(workflowOptions);
                }*/

                if (!deviceConnected)
                {
                    //linkRequestResponse = RequestHelper.LinkRequestResponseError(linkRequest, ErrorCodes.NoDevice, ErrorDescriptions.DeviceNotFound);

                    object response = JsonConvert.SerializeObject(linkRequestResponse);
                    //_ = Controller.LoggingClient.LogInfoAsync($"Sending to Listener");
                    //await Controller.Connector.Publish(response, new TopicOption[] { TopicOption.Servicer }).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                //_ = Controller.LoggingClient.LogErrorAsync($"{e.Message} {linkRequest}");
                //if (Controller.Connector != null)
                //{
                //    await Controller.Connector.Publish(RequestHelper.LinkRequestResponseError(null, ErrorCodes.DalError, e.Message), new TopicOption[] { TopicOption.Servicer }).ConfigureAwait(false);
                //}
            }
        }
    }
}