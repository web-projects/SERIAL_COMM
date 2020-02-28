using System;
using System.Collections.Generic;
using System.Text;

namespace SERIAL_COMM.CommandLayer
{
    internal class VIPAResponseHandlers
    {
        public DeviceManager.ResponseTagsHandlerDelegate responsetagshandler { get; set; }
        public DeviceManager.ResponseTaglessHandlerDelegate responsetaglesshandler { get; set; }

        public DeviceManager.ResponseCLessHandlerDelegate responsecontactlesshandler { get; set; }
    }
}