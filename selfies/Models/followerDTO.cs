using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace selfies.Models
{
    public class followerDTO
    {
        public int id { get; set; }
        public int followerId { get; set; }
        public int followeeId { get; set; }
        public int active { get; set; }
        public string mostRecentSnap { get; set; }

        public virtual handle followeeHandle { get; set; }
        public virtual handle followerHandle { get; set; }
    }
}