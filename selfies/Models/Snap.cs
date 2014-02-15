using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace selfies.Models
{
    public class Snap
    {
        public string toGuid { get; set; }
        public string groupKey { get; set; }
        public string caption { get; set; }
        public string location { get; set; }
        public string font { get; set; }
        public string textColor { get; set; }
    }
}