using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace XO.Requests
{
    public partial class LinkDeviceActionRequest
    {
        public LinkDeviceActionType? DeviceAction { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LinkDeviceActionType
    {
        GetStatus,
        AbortCommand,
        ResetCommand,
        GetIdentifier,
    }
}
