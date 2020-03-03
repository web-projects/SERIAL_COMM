using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Interfaces;
using Devices.Common;
using Devices.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DEVICE_CORE.StateMachine.State.Actions
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

            try
            {
                availableCardDevices = Controller.DevicePluginLoader.FindAvailableDevices(pluginPath);
                for (int i = availableCardDevices.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(availableCardDevices[i].ManufacturerConfigID, DeviceType.NoDevice.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    bool success = false;
                    try
                    {
                        List<DeviceInformation> deviceInformation = availableCardDevices[i].DiscoverDevices();

                        foreach (var deviceInfo in deviceInformation)
                        {
                            DeviceConfig deviceConfig = new DeviceConfig()
                            {
                                Valid = true
                            };

                            ICardDevice device = availableCardDevices[i].Clone() as ICardDevice;

                            //device.DeviceEventOccured += Controller.DeviceEventReceived;
                            device.Probe(deviceConfig, deviceInfo, out success);

                            if (success)
                            {
                                discoveredCardDevices.Add(device);
                            }
                        }
                    }
                    catch
                    {
                        //availableCardDevices[i].DeviceEventOccured -= Controller.DeviceEventReceived;

                        // Consume failures
                        if (success)
                        {
                            success = false;
                        }
                    }
                }
            }
            catch
            {
                availableCardDevices = new List<ICardDevice>();
            }




            //List<ICardDevice> availableCardDevices = null;
            //List<ICardDevice> validatedCardDevices = null;
            //List<ICardDevice> discoveredCardDevices = null;

            // TODO: Jon, please ensure you add some more test cases here to cover the other devices
            // that are not covered already in the unit test.
            /*try
            {
                availableCardDevices = Controller.DevicePluginLoader.FindAvailableDevices(pluginPath);
                validatedCardDevices = new List<ICardDevice>();
                discoveredCardDevices = new List<ICardDevice>();
                validatedCardDevices.AddRange(availableCardDevices);
                for (int i = validatedCardDevices.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(availableCardDevices[i].ManufacturerConfigID, DeviceType.NoDevice.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    bool success = false;
                    try
                    {
                        List<DeviceInformation> deviceInformation = availableCardDevices[i].DiscoverDevices();

                        foreach (var deviceInfo in deviceInformation)
                        {
                            DeviceConfig deviceConfig = new DeviceConfig()
                            {
                                Valid = true
                            };

                            ICardDevice device = availableCardDevices[i].Clone() as ICardDevice;

                            device.DeviceEventOccured += Controller.DeviceEventReceived;
                            device.Probe(deviceConfig, deviceInfo, out success);

                            if (success)
                            {
                                discoveredCardDevices.Add(device);
                            }
                        }
                    }
                    catch
                    {
                        validatedCardDevices[i].DeviceEventOccured -= Controller.DeviceEventReceived;

                        // Consume failures
                        if (success)
                        {
                            success = false;
                        }
                    }

                    if (validatedCardDevices[i].Name == "Simulator Device")
                    {
                        success = true;
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
                availableCardDevices = validatedCardDevices = new List<ICardDevice>();
            }

            // TODO: Utilizing the plugins we must check our configuration to determine
            // the preferred device we are interested in working with.
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

                if (discoveredCardDevices?.Count > 1)
                {
                    Controller.SetTargetDevices(discoveredCardDevices);
                }
                else if (discoveredCardDevices?.Count == 1)
                {
                    Controller.SetTargetDevice(discoveredCardDevices[0]);
                }
            }
            else if (Controller.Configuration?.Simulator?.AllowSimulator ?? false)
            {
                int index = availableCardDevices.FindIndex(x => x.GetType().Name.Equals(StringValueAttribute.GetStringValue(DeviceType.NoDevice)));
                if (index >= 0)
                {
                    Controller.SetTargetDevice(availableCardDevices[index]);
                }
            }

            // set NoDevice when devices were not found
            if (validatedCardDevices.Count == 0 || discoveredCardDevices.Count == 0)
            {
                Controller.LoggingClient.LogInfoAsync("No device detected.");

                int index = availableCardDevices.FindIndex(x => x.GetType().Name.Equals(StringValueAttribute.GetStringValue(DeviceType.NoDevice)));
                if (index >= 0)
                {
                    List<DeviceInformation> deviceInformation = availableCardDevices[index].DiscoverDevices();

                    DeviceConfig deviceConfig = new DeviceConfig()
                    {
                        Valid = true
                    };

                    availableCardDevices[index].Probe(deviceConfig, deviceInformation[0], out bool success);

                    Controller.SetTargetDevice(availableCardDevices[index]);
                }
            }

            if (Controller.TargetDevice != null)
            {
                Controller.LoggingClient.LogInfoAsync($"Device found: name='{Controller.TargetDevice.Name}', model={Controller.TargetDevice.DeviceInformation?.Model}, " +
                    $"serial={Controller.TargetDevice.DeviceInformation?.SerialNumber}");
                Controller.TargetDevice.DeviceSetIdle();
            }
            else if (Controller.TargetDevices != null)
            {
                foreach (var device in Controller.TargetDevices)
                {
                    Controller.LoggingClient.LogInfoAsync($"Device found: name='{device.Name}', model={device.DeviceInformation.Model}, " +
                        $"serial={device.DeviceInformation.SerialNumber}");
                    device.DeviceSetIdle();
                }
            }
            else
            {
                Controller.LoggingClient.LogInfoAsync("Unable to find a valid device to connect to.");
                LastException = new StateException("Unable to find a valid device to connect to.");
                _ = Error(this);
            }
            */

            if (discoveredCardDevices?.Count > 0)
            {
                Controller.SetTargetDevices(discoveredCardDevices);
            }

            if (Controller.TargetDevices != null)
            {
                foreach (var device in Controller.TargetDevices)
                {
                    //Controller.LoggingClient.LogInfoAsync($"Device found: name='{device.Name}', model={device.DeviceInformation.Model}, " +
                    //    $"serial={device.DeviceInformation.SerialNumber}");
                    Console.WriteLine($"Device found: name='{device.Name}', model={device.DeviceInformation.Model}, " +
                        $"serial={device.DeviceInformation.SerialNumber}");
                    device.DeviceSetIdle();
                }
            }
            else
            {
                //Controller.LoggingClient.LogInfoAsync("Unable to find a valid device to connect to.");
                Console.WriteLine("Unable to find a valid device to connect to.");
                LastException = new StateException("Unable to find a valid device to connect to.");
                _ = Error(this);
            }

            _ = Complete(this);

            return Task.CompletedTask;
        }
    }
}
