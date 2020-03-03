namespace Devices.Verifone.VIPA
{
    public class VIPAResponseHandlers
    {
        public VIPADevice.ResponseTagsHandlerDelegate responsetagshandler { get; set; }
        public VIPADevice.ResponseTaglessHandlerDelegate responsetaglesshandler { get; set; }

        public VIPADevice.ResponseCLessHandlerDelegate responsecontactlesshandler { get; set; }
    }
}