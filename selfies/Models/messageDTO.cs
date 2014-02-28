using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace selfies.Models
{
    public class messageDTO
    {

        public int id { get; set; }
        public int fromHandleId { get; set; }
        public int threadId { get; set; }
        public System.DateTime sentDate { get; set; }
        public int active { get; set; }
        public int anon { get; set; }
        public int toHandleSeen { get; set; }
        public string messageText { get; set; }
        public string toKey { get; set; }

        public virtual thread thread { get; set; }
        public virtual handle fromHandle { get; set; }
        public virtual handle toHandle { get; set; }

    }
}