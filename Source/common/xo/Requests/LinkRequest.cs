using System;
using System.Collections.Generic;

namespace XO.Requests
{
    public partial class LinkRequest
    {
        public string MessageID { get; set; }
        public List<LinkActionRequest> Actions { get; set; }
    }
}
