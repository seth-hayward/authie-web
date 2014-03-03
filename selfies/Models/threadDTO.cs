using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace selfies.Models
{
    public class threadDTO
    {

        public int id { get; set; }
        public int fromHandleId { get; set; }
        public int toHandleId { get; set; }
        public string groupKey { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public int active { get; set; }
        public string caption { get; set; }
        public Nullable<int> authorizeRequest { get; set; }
        public Nullable<int> toHandleSeen { get; set; }
        public Nullable<int> hearts { get; set; }
        public int uploadSuccess { get; set; }
        public string location { get; set; }
        public string font { get; set; }
        public string textColor { get; set; }

        public virtual handle fromHandle { get; set; }
        public virtual handle toHandle { get; set; }

        public DateTime lastMessageDate { get; set; }

        public List<handle> convos { get; set; }

    }
}