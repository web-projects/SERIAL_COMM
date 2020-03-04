namespace DEVICE_CORE.State.SubWorkflows
{
    internal class SubStateActionLaunchRules
    {
        public bool RequestCancellationToken { get; set; }
        public bool DisableRequestPreProcessing { get; set; }
        public bool DisableDeviceEventPreProcessing { get; set; }
    }
}
