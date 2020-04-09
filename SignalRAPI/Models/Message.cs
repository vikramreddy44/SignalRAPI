using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRAPI
{
    public class Message
    {
        public string clientuniqueid { get; set; }
        public string type { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        public string groupName { get; set; }
    }
}