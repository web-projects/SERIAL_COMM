using Config;
using StateMachine.State.Enums;
using StateMachine.State.Interfaces;
using Devices.Common;
using Devices.Common.Helpers;
using Devices.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using XO.Responses;
using StateMachine.Cancellation;
using Devices.Common.Constants;

namespace StateMachine.State.Actions
{
    internal class DeviceInitializeDeviceCommunicationStateAction : DeviceBaseStateAction
    {
        public override DeviceWorkflowState WorkflowStateType => DeviceWorkflowState.InitializeDeviceCommunication;

        public DeviceInitializeDeviceCommunicationStateAction(IDeviceStateController _) : base(_) { }

        public override Task DoWork()
        {
            string pluginPath = Controller.PluginPath;
            List<ICardDevice> availableCardDevices = null;
            List<ICardDevice> discoveredCardDevices = null;
            List<ICardDevice> validatedCardDevices = null;

            try
            {
                availableCardDevices = discoveredCardDevices = Controller.DevicePluginLoader.FindAvailableDevices(pluginPath);

                if (discoveredCardDevices.Count > 0)
                {
                    DeviceSection deviceSection = Controller.Configuration;
                    foreach (var device in discoveredCardDevices)
                    {
                        switch (device.ManufacturerConfigID)
                        {
                            case "IdTech":
                            {
                                device.SortOrder = deviceSection.IdTech.SortOrder;
                                break;
                            }

                            case "Verifone":
                            {
                                device.SortOrder = deviceSection.Verifone.SortOrder;
                                break;
                            }

                            case "Simulator":
                            {
                                device.SortOrder = deviceSection.Simulator.SortOrder;
                                break;
                            }
                        }
                    }
                    discoveredCardDevices.RemoveAll(x => x.SortOrder == -1);

                    if (discoveredCardDevices?.Count > 1)
                    {
                        discoveredCardDevices.Sort();
                    }
                }

                // Probe validated devices
                validatedCardDevices = new List<ICardDevice>();

                for (int i = discoveredCardDevices.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(discoveredCardDevices[i].ManufacturerConfigID, DeviceType.NoDevice.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    bool success = false;
                    try
                    {
                        List<DeviceInformation> deviceInformation = discoveredCardDevices[i].DiscoverDevices();

                        foreach (var deviceInfo in deviceInformation)
                        {
                            DeviceConfig deviceConfig = new DeviceConfig()
                            {
                                Valid = true
                            };
                            SerialDeviceConfig serialConfig = new SerialDeviceConfig
                            {
                                CommPortName = Controller.Configuration.DefaultDevicePort
                            };
                            deviceConfig.SetSerialDeviceConfig(serialConfig);

                            ICardDevice device = discoveredCardDevices[i].Clone() as ICardDevice;

                            device.DeviceEventOccured += Controller.DeviceEventReceived;
 
                            // Device powered on status capturing: free up the com port and try again.
                            // This occurs when a USB device repowers the USB interface and the virtual port is open.
                            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                            IDeviceCancellationBroker cancellationBroker = Controller.GetCancellationBroker();
                            var timeoutPolicy = cancellationBroker.ExecuteWithTimeoutAsync<List<LinkErrorValue>>(
                                _ => device.Probe(deviceConfig, deviceInfo, out success),
                                Timeouts.DALGetStatusTimeout,
                                CancellationToken.None);

                            if (timeoutPolicy.Result.Outcome == Polly.OutcomeType.Failure)
                            {
                                Console.WriteLine($"Unable to obtain device status for - '{device.Name}'.");
                                device.DeviceEventOccured -= Controller.DeviceEventReceived;
                                device?.Dispose();
                                LastException = new StateException("Unable to find a valid device to connect to.");
                                _ = Error(this);
                                return Task.CompletedTask;
                            }
                            else if (success)
                            {
                                discoveredCardDevices.Add(device);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"device: exception='{e.Message}'");

                        discoveredCardDevices[i].DeviceEventOccured -= Controller.DeviceEventReceived;

                        // Consume failures
                        if (success)
                        {
                            success = false;
                        }
                    }

                    if (success)
                    {
                        continue;
                    }

                    validatedCardDevices.RemoveAt(i);
                }
            }
            catch
            {
                availableCardDevices = new List<ICardDevice>();
            }

            if (validatedCardDevices?.Count > 0)
            {
                Controller.SetTargetDevices(validatedCardDevices);
            }
            else
            {
                Controller.SetTargetDevices(null);
            }

            if (Controller.TargetDevices != null)
            {
                foreach (var device in Controller.TargetDevices)
                {
                    //Controller.LoggingClient.LogInfoAsync($"Device found: name='{device.Name}', model={device.DeviceInformation.Model}, " +
                    //    $"serial={device.DeviceInformation.SerialNumber}");
                    Console.WriteLine($"Device found: name='{device.Name}', model='{device.DeviceInformation.Model}', " +
                        $"serial='{device.DeviceInformation.SerialNumber}'");
                    device.DeviceSetIdle();
                }
            }
            else
            {
                //Controller.LoggingClient.LogInfoAsync("Unable to find a valid device to connect to.");
                Console.WriteLine("Unable to find a valid device to connect to.");
                LastException = new StateException("Unable to find a valid device to connect to.");
                _ = Error(this);
                return Task.CompletedTask;
            }

            _ = Complete(this);

            return Task.CompletedTask;
        }
    }
}
