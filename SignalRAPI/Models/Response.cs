using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRAPI
{
    public class Response
    {
        public string Status { set; get; }
        public string Message { set; get; }

        public object obj { set; get; }
    }
}