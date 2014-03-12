using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace selfies.Models
{
    public class Chat
    {
        public string groupKey { get; set; }
        public string message { get; set; }
        public string toKey { get; set; }
    }
}