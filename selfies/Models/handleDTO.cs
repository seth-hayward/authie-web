using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace selfies.Models
{
    public class handleDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public string userGuid { get; set; }
        public Nullable<int> active { get; set; }
        public string publicKey { get; set; }
        public string tagLine { get; set; }
        public string mostRecentPublicSnap { get; set; }
    }
}