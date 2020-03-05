using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using Devices.Common.Helpers;
using System;
using System.Threading.Tasks;
using XO.Requests;

namespace DEVICE_CORE.StateMachine.State.Actions
{
    internal class DeviceManageStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.Manage;

        public DeviceManageStateAction(IDeviceStateController _) : base(_) { }

        static private LinkDeviceActionType lastDeviceAction = LinkDeviceActionType.GetStatus;

        public override bool DoDeviceDiscovery()
        {
            LastException = new StateException("device recovery is needed");
            _ = Error(this);
            return true;
        }

        private async void PostRequest()
        {
            if (Controller.TargetDevices != null)
            {
                await Task.Delay(10240);

                // DEVICE RESET COMMAND
                LinkRequest linkRequest = new LinkRequest()
                {
                    MessageID = RandomGenerator.BuildRandomString(12),
                    Actions = new System.Collections.Generic.List<LinkActionRequest>()
                    {
                        new LinkActionRequest()
                        {
                            Action = LinkAction.DALAction,
                            DeviceActionRequest = new LinkDeviceActionRequest()
                            {
                                DeviceAction = lastDeviceAction
                            },
                            DeviceRequest = new LinkDeviceRequest()
                            {
                                DeviceIdentifier = new XO.Device.LinkDeviceIdentifier()
                                {
                                    Manufacturer = "Simulator",
                                    Model = "SimCity",
                                    SerialNumber = "CEEEDEADBEEF"
                                }
                            }
                        }
                    }
                };
                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine($"REQUEST: {lastDeviceAction}");
                Controller.SendDeviceCommand(Newtonsoft.Json.JsonConvert.SerializeObject(linkRequest));
                lastDeviceAction += 1;
                if (lastDeviceAction >= LinkDeviceActionType.GetIdentifier)
                {
                    lastDeviceAction = LinkDeviceActionType.GetStatus;
                }
            }
        }

        public override Task DoWork()
        {
            PostRequest();

            return Task.CompletedTask;
        }


        public override void RequestReceived(LinkRequest request)
        {
            Controller.SaveState(request);

            _ = Complete(this);
        }
    }
}