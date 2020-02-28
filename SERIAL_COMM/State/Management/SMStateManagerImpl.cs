using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ninject;
using Newtonsoft.Json;
using SERIAL_COMM.Connection.Interfaces;
using SERIAL_COMM.Helpers;
using SERIAL_COMM.State.Enums;
using SERIAL_COMM.State.Interfaces;
using SERIAL_COMM.State.Actions;
using SERIAL_COMM.State.Actions.Controllers;

namespace SERIAL_COMM.State.Management
{
    //internal class SMStateManagerImpl : ISMStateController, ISMStateManager, IInitializable, ISubWorkflowHook
    internal class SMStateManagerImpl : IInitializable, ISMStateManager
    {
        private bool disposed;

        //[Inject]
        //public IListenerConnectorProvider ListenerConnectorProvider { get; set; }

        //[Inject]
        //public IDALConfigurationProvider DALConfigurationProvider { get; set; }

        //[Inject]
        //public IDevicePluginLoader DevicePluginLoader { get; set; }

        //[Inject]
        //public ILoggingServiceClientProvider LoggingServiceClientProvider { get; set; }

        [Inject]
        public ISMStateActionControllerProvider SMStateActionControllerProvider { get; set; }

        //[Inject]
        //public IControllerVisitorProvider ControllerVisitorProvider { get; set; }

        //[Inject]
        //public ISubStateManagerProvider SubStateManagerProvider { get; set; }

        [Inject]
        public ISerialPortMonitor SerialPortMonitor { get; set; }

        //[Inject]
        //public IDeviceCancellationBrokerProvider DeviceCancellationBrokerProvider { get; set; }

        //public DeviceSection Configuration { get; private set; }

        //public ILoggingServiceClient LoggingClient { get; private set; }

        //public IListenerConnector Connector { get; private set; }

        //public List<ICardDevice> AvailableCardDevices { get; private set; } = new List<ICardDevice>();

        public string PluginPath { get; private set; }

        //public ICardDevice TargetDevice { get; private set; }

        //public List<ICardDevice> TargetDevices { get; private set; }

        //public DeviceEventHandler DeviceEventReceived { get; set; }

        public ComPortEventHandler ComPortEventReceived { get; set; }

        //public PriorityQueue<PriorityQueueDeviceEvents> PriorityQueue { get; set; }

        public StateActionRules StateActionRules { get; private set; }

        public bool NeedsDeviceRecovery { get; set; }

        public bool DALListenerIsOnline { get; set; }

        private static bool subscribed { get; set; }
        //private ISMSubStateController subStateController;
        private ISMStateAction currentStateAction;
        private ISMStateActionController stateActionController;
        private readonly Stack<object> savedStackState = new Stack<object>();

        public event OnStateChange StateChange;
        public event OnWorkflowStopped WorkflowStopped;
        //public event OnRequestReceived RequestReceived;

        public SMStateManagerImpl()
        {
            //PriorityQueue = new PriorityQueue<PriorityQueueDeviceEvents>();
        }

        public void Initialize()
        {
            //DeviceEventReceived = OnDeviceEventReceived;
            ComPortEventReceived = OnComPortEventReceived;

            SerialPortMonitor.ComportEventOccured += OnComPortEventReceived;
            SerialPortMonitor.StartMonitoring();

            StateActionRules = new StateActionRules();

            //DALConfigurationProvider.InitializeConfiguration();
            //Configuration = DALConfigurationProvider.GetAppConfig();
            //LoggingClient = LoggingServiceClientProvider.GetLoggingServiceClient();
            //Connector = ListenerConnectorProvider.GetConnector(DALConfigurationProvider.GetConfiguration());

            stateActionController = SMStateActionControllerProvider.GetStateActionController(this);

            //InitializeConnectorEvents();
        }

        public void SetPluginPath(string pluginPath) => (PluginPath) = (pluginPath);

        //public void SetTargetDevice(ICardDevice targetDevice)
        //{
        //    if (TargetDevices != null)
        //    {
        //        foreach (var device in TargetDevices)
        //        {
        //            device?.Dispose();
        //        }
        //    }
        //    TargetDevice?.Dispose();
        //    TargetDevice = targetDevice;
        //}

        //public void SetTargetDevices(List<ICardDevice> targetDevices)
        //{
        //    TargetDevice?.Dispose();
        //    if (TargetDevices != null)
        //    {
        //        foreach (var device in TargetDevices)
        //        {
        //            device?.Dispose();
        //        }
        //    }
        //    TargetDevices = targetDevices;
        //}

        //public void SetPublishEventHandlerAsTask()
        //{
        //    if (TargetDevice != null)
        //    {
        //        NeedsDeviceRecovery = false;
        //        TargetDevice.PublishEvent += PublishEventHandlerAsTask;
        //        TargetDevice.DeviceEventOccured += OnDeviceEventReceived;
        //    }
        //    else if (TargetDevices != null)
        //    {
        //        foreach (var device in TargetDevices)
        //        {
        //            NeedsDeviceRecovery = false;
        //            device.PublishEvent += PublishEventHandlerAsTask;
        //            device.DeviceEventOccured += OnDeviceEventReceived;
        //        }
        //    }
        //}

        public void SaveState(object stateObject) => savedStackState.Push(stateObject);

        public async Task Recovery(ISMStateAction state, object stateObject)
        {
            SaveState(stateObject);
            await AdvanceActionWithState(state);
        }

        //public void Hook(IDALSubStateController controller) => subStateController = (controller);

        //public void UnHook() => subStateController = (null);

        //public IControllerVisitorProvider GetCurrentVisitorProvider() => ControllerVisitorProvider;

        //public IDeviceCancellationBroker GetCancellationBroker() => DeviceCancellationBrokerProvider.GetDeviceCancellationBroker();

        //public ISubStateManagerProvider GetSubStateManagerProvider() => SubStateManagerProvider;

        //protected void RaiseOnRequestReceived(string data)
        //{
        //    try
        //    {
        //        LogMessage("Request received from", data);
        //        LinkRequest linkRequest = JsonConvert.DeserializeObject<LinkRequest>(data.ToString());
        //        RequestReceived?.Invoke(linkRequest);
        //    }
        //    catch (Exception e)
        //    {
        //        LogError(e.Message, data);
        //        if (Connector != null)
        //        {
        //            Connector.Publish(LinkRequestResponseError(null, "DALError", e.Message), new TopicOption[] { TopicOption.Servicer }).ConfigureAwait(false);
        //        }
        //    }
        //}

        protected void RaiseOnWorkflowStopped(SMWorkflowStopReason reason) => WorkflowStopped?.Invoke(reason);

        //private void OnDeviceEventReceived(DeviceEvent deviceEvent, DeviceInformation deviceInformation)
        //{
        //    if (currentStateAction.WorkflowStateType == SubWorkflowIdleState)
        //    {
        //        if (subStateController != null)
        //        {
        //            IDALSubStateManager subStateManager = subStateController as IDALSubStateManager;
        //            subStateManager.DeviceEventReceived(deviceEvent, deviceInformation);
        //        }
        //    }
        //}

        private void OnComPortEventReceived(PortEventType comPortEvent, string portNumber)
        {
            if (comPortEvent == PortEventType.Insertion)
            {
                //_ = LoggingClient.LogInfoAsync($"Comport Plugged. ComportNumber '{portNumber}'. Detecting a new connection...");
                Console.WriteLine($"Comport Plugged. ComportNumber '{portNumber}'. Detecting a new connection...");
            }
            else if (comPortEvent == PortEventType.Removal)
            {
                //if (TargetDevices != null)
                //{
                //    // dispose of all connections so that device recovery re-validates them
                //    foreach (var device in TargetDevices)
                //    {
                //        if (string.Equals(portNumber, device.DeviceInformation.ComPort, StringComparison.CurrentCultureIgnoreCase))
                //        {
                //            _ = LoggingClient.LogInfoAsync($"Comport unplugged. ComportNumber '{portNumber}', " +
                //                $"DeviceType '{device.ManufacturerConfigID}', SerialNumber '{device.DeviceInformation?.SerialNumber}'");
                //        }
                //        device.Dispose();
                //    }
                //}
                //else
                //{
                    //_ = LoggingClient.LogInfoAsync($"Comport unplugged. ComportNumber '{portNumber}', " +
                    //    $"DeviceType '{TargetDevice?.ManufacturerConfigID}', SerialNumber '{TargetDevice?.DeviceInformation?.SerialNumber}'");
                    //TargetDevice?.Dispose();
                //}

                Console.WriteLine($"Comport unplugged. ComportNumber '{portNumber}'");
            }
            else
            {
                //_ = LoggingClient.LogInfoAsync($"Comport Event '{comPortEvent}' is not implemented ");
                Console.WriteLine($"Comport Event '{comPortEvent}' is not implemented ");
            }

            //LoggingClient.LogInfoAsync($"Device recovery in progress...");
            Console.WriteLine($"Device recovery in progress...");

            if (currentStateAction.WorkflowStateType == SMWorkflowState.Manage)
            {
                currentStateAction.DoDeviceDiscovery();
            }
            //else
            //{
            //    StateActionRules.NeedsDeviceRecovery = true;

            //    if (subStateController != null)
            //    {
            //        IDALSubStateManager subStateManager = subStateController as IDALSubStateManager;
            //        subStateManager.ComportEventReceived(comPortEvent, portNumber);
            //    }
            //}
        }

        private void OnQueueEventOcurred()
        {
            //TODO: EventChecker will handle PriorityQueue event dequeuing
        }

        private void LogError(string message, object data)
        {
            string messageId = "Unknown";
            try
            {
                //var request = JsonConvert.DeserializeObject<LinkRequest>(data as string);
                //if (!string.IsNullOrWhiteSpace(request?.MessageID))
                //{
                //    messageId = request.MessageID;
                //}
            }
            finally
            {
                //_ = LoggingClient.LogInfoAsync($"Error from MessageId: '{messageId}':{message}").ConfigureAwait(false);
                Console.WriteLine($"Error from MessageId: '{messageId}':{message}");
            }
        }

        //private LinkRequest LinkRequestResponseError(LinkRequest linkRequest, string codeType, string description)
        //{
        //    if (linkRequest == null)
        //    {
        //        linkRequest = new LinkRequest()
        //        {
        //            LinkObjects = new LinkRequestIPA5Object() { LinkActionResponseList = new List<LinkActionResponse>() },
        //            Actions = new List<LinkActionRequest>() { new LinkActionRequest() { MessageID = "Unknown" } }
        //        };
        //    }
        //    linkRequest.LinkObjects.LinkActionResponseList.Add(new LinkActionResponse()
        //    {
        //        MessageID = linkRequest.Actions[0].MessageID,
        //        Errors = new List<LinkErrorValue>() { new LinkErrorValue() { Code = codeType, Type = codeType, Description = description } }
        //    });
        //    return linkRequest;
        //}

        //private void LogMessage(string sendReceive, object data)
        //{
        //    var request = JsonConvert.DeserializeObject<LinkRequest>(data as string);
        //    string messageId = null;
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(request?.MessageID))
        //        {
        //            messageId = request.MessageID;
        //        }
        //    }
        //    finally
        //    {
        //        _ = LoggingClient.LogInfoAsync($"Request of MessageID: '{messageId}' {sendReceive} Listener.").ConfigureAwait(false);
        //    }
        //}

        //private void InitializeConnectorEvents()
        //{
        //    Connector.MessageReceived += ListenerConnector_MessageReceived;
        //    Connector.OfflineConnectivity += ListenerConnector_OfflineConnectivity;
        //    Connector.OnlineConnectivity += ListenerConnector_OnlineConnectivity;
        //    Connector.ChannelClient.ChannelConnected += ChannelClient_ChannelConnected;
        //    Connector.ChannelClient.ChannelDisconnected += ChannelClient_ChannelDisconnected;
        //    Connector.ChannelClient.ChannelReconnected += ChannelClient_ChannelReconnected;
        //}

        private void DestroyComportMonitoring()
        {
            if (SerialPortMonitor != null)
            {
                SerialPortMonitor.ComportEventOccured -= OnComPortEventReceived;
                SerialPortMonitor.StopMonitoring();
            }
        }

        //private void DestroyConnectorEvents()
        //{
        //    if (Connector != null)
        //    {
        //        Connector.MessageReceived -= ListenerConnector_MessageReceived;
        //        Connector.OfflineConnectivity -= ListenerConnector_OfflineConnectivity;
        //        Connector.OnlineConnectivity -= ListenerConnector_OnlineConnectivity;

        //        if (Connector.ChannelClient != null)
        //        {
        //            Connector.ChannelClient.ChannelConnected -= ChannelClient_ChannelConnected;
        //            Connector.ChannelClient.ChannelDisconnected -= ChannelClient_ChannelDisconnected;
        //            Connector.ChannelClient.ChannelReconnected -= ChannelClient_ChannelReconnected;
        //        }
        //    }
        //}

        //private void DisconnectFromListener()
        //{
        //    // TODO: Jon, please consider moving this timeout into your configuration file :).
        //    Connector.Unsubscribe().Wait(5000);
        //    Connector.Dispose();
        //    Connector = null;
        //}

        //private void ChannelClient_ChannelReconnected()
        //{
        //    _ = LoggingClient.LogInfoAsync("DAL is currently reconnected to Listener.");
        //}

        //internal void ChannelClient_ChannelDisconnected(Guid channelId)
        //{
        //    subscribed = false;

        //    _ = LoggingClient.LogInfoAsync($"DAL is currently disconnected from Listener with client id {GetShortClientId(channelId)}");
        //}

        //internal void ChannelClient_ChannelConnected(Guid channelId)
        //{
        //    _ = LoggingClient.LogInfoAsync($"DAL is currently connected to the Listener with client id {GetShortClientId(channelId)}.");

        //    if (!subscribed)
        //    {
        //        Connector.Subscribe(new TopicOption[] { TopicOption.DAL });

        //        subscribed = true;

        //        _ = LoggingClient.LogInfoAsync("DAL is subscribed to the Listener.");
        //    }
        //}

        //private void ListenerConnector_OnlineConnectivity()
        //{
        //    DALListenerIsOnline = true;
        //    _ = LoggingClient.LogInfoAsync("Network connectivity is online.");
        //}

        //private void ListenerConnector_OfflineConnectivity()
        //{
        //    DALListenerIsOnline = false;
        //    _ = LoggingClient.LogInfoAsync("Network connectivity has gone offline.");
        //}

        //private void ListenerConnector_MessageReceived(Listener.Common.Packets.ListenerPacketHeader header, object message)
        //=> RaiseOnRequestReceived(message as string);

        private void LogStateChange(SMWorkflowState oldState, SMWorkflowState newState)
            //=> _ = LoggingClient.LogInfoAsync($"Sub-Workflow State change from '{oldState}' to '{newState}' detected.");
            => Console.WriteLine($"Sub-Workflow State change from '{oldState}' to '{newState}' detected.");

        //internal void PublishEventHandler(LinkEventResponse.EventTypeType eventType, LinkEventResponse.EventCodeType eventCode,
        //    List<LinkDeviceResponse> devices, LinkRequest request, string message)
        //{
        //    string sessionId = request.Actions?[0]?.SessionID;
        //    if (!string.IsNullOrWhiteSpace(sessionId))
        //    {
        //        try
        //        {
        //            var eventToPublish = ComposeEvent(sessionId, eventType, eventCode, devices, request, new List<string>() { (message ?? string.Empty) }, online: DALListenerIsOnline);
        //            string jsonToPublish = JsonConvert.SerializeObject(eventToPublish);
        //            Connector.Publish(jsonToPublish, new string[] { TopicOption.Event.ToString() });
        //        }
        //        catch (Exception xcp)
        //        {
        //            LoggingClient.LogErrorAsync(xcp.Message);
        //        }
        //    }
        //}

        //internal void PublishEventHandlerAsTask(LinkEventResponse.EventTypeType eventType, LinkEventResponse.EventCodeType eventCode,
        //    List<LinkDeviceResponse> devices, LinkRequest request, string message)
        //{
        //    _ = Task.Run(() => PublishEventHandler(eventType, eventCode, devices, request, message)).ConfigureAwait(false);
        //}

        //public LinkResponse ComposeEvent(string sessionId, LinkEventResponse.EventTypeType eventType,
        //    LinkEventResponse.EventCodeType eventCode, List<LinkDeviceResponse> devices, LinkRequest request, List<string> eventData,
        //    bool online = false)
        //{
        //    var eventResponse = new LinkResponse()
        //    {
        //        MessageID = request?.MessageID,
        //        Responses = new List<LinkActionResponse>(1)
        //         {
        //              new LinkActionResponse()
        //              {
        //                  MessageID = request.Actions?[0].MessageID,
        //                  DALResponse = new LinkDALResponse()
        //                   {
        //                        Devices = devices,
        //                        DALIdentifier = request?.Actions?[0].DALRequest?.DALIdentifier ?? DalIdentifier.GetDALIdentifier(),
        //                        OnlineStatus = online
        //                   },
        //                    EventResponse = new LinkEventResponse()
        //                    {
        //                         EventCode = eventCode.ToString(),
        //                         EventType = eventType.ToString(),
        //                         EventID = Guid.NewGuid(),
        //                         EventData = eventData == null ? null : eventData.ToArray(),
        //                         OrdinalID = 0
        //                     },
        //                    SessionResponse = new LinkSessionResponse()
        //                    {
        //                        SessionID = sessionId
        //                    }
        //               }
        //         }
        //    };

        //    if (eventResponse.Responses[0].DALResponse == null)
        //    {
        //        eventResponse.Responses[0].DALResponse = new LinkDALResponse();
        //    }
        //    if (eventResponse.Responses[0].DALResponse.DALIdentifier == null)
        //    {
        //        eventResponse.Responses[0].DALResponse.DALIdentifier = request.Actions?[0].LinkObjects?.ActionResponse?.DALResponse?.DALIdentifier;
        //    }
        //    if (eventResponse.Responses[0].DALResponse.Devices?[0] == null)
        //    {
        //        eventResponse.Responses[0].DALResponse.Devices = request.Actions?[0].LinkObjects?.ActionResponse?.DALResponse?.Devices ?? new List<LinkDeviceResponse>() { null };
        //    }

        //    return eventResponse;
        //}

        #region --- state machine management ---

        public Task Complete(ISMStateAction state) => AdvanceStateActionTransition(state);

        public void LaunchWorkflow() => stateActionController.GetNextAction(SMWorkflowState.None).DoWork();

        public void StopWorkflow()
        {
            if (!disposed)
            {
                disposed = true;

                //_ = LoggingClient.LogInfoAsync("Currently shutting down DAL Workflow...");
                Console.WriteLine(("Currently shutting down DAL Workflow...");

                currentStateAction?.Dispose();

                DestroyComportMonitoring();
                //DestroyConnectorEvents();
                //DisconnectFromListener();

                ExecuteFinalState();
            }
        }

        private void ExecuteFinalState()
        {
            using ISMStateAction lastAction = stateActionController.GetFinalState();
            lastAction.DoWork().Wait(2000);
        }

        private async Task AdvanceStateActionTransition(ISMStateAction oldState)
        {
            ISMStateAction newState = stateActionController.GetNextAction(oldState);

            if (savedStackState.Count > 0)
            {
                newState.SetState(savedStackState.Pop());
            }

            oldState.Dispose();

            currentStateAction = newState;

            LogStateChange(oldState.WorkflowStateType, newState.WorkflowStateType);

            await newState.DoWork();
        }

        private async Task AdvanceActionWithState(ISMStateAction oldState)
        {
            ISMStateAction newState = stateActionController.GetNextAction(oldState);

            if (savedStackState.Count > 0)
            {
                newState.SetState(savedStackState.Pop());
            }

            oldState.Dispose();

            currentStateAction = newState;

            RaiseStateChange(oldState.WorkflowStateType, newState.WorkflowStateType);

            if (StateActionRules.NeedsDeviceRecovery)
            {
                if (currentStateAction.DoDeviceDiscovery())
                {
                    StateActionRules.NeedsDeviceRecovery = false;
                }
            }

            await newState.DoWork();
        }

        protected void RaiseStateChange(SMWorkflowState oldState, SMWorkflowState newState)
                    => StateChange?.Invoke(oldState, newState);

        public async Task Error(ISMStateAction state)
        {
            if (state.WorkflowStateType == SMWorkflowState.None)
            {
                // TODO: Modify this workflow so that it follows the pattern and simply loops back around
                // to the same final state. In this way, we would run through Shutdown once and then simply
                // decide at that point to stop the workflow because we have no more states to advance to.
                StopWorkflow();
                RaiseOnWorkflowStopped(state.StopReason);
            }
            else
            {
                await AdvanceActionWithState(state);
            }
        }

        public void Dispose() => StopWorkflow();

        #endregion --- state machine management ---
    }
}
