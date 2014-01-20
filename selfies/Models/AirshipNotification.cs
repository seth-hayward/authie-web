using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace selfies.Models
{
    public class AirshipNotification
    {


        // should be using netwonsoft for this!!!!!

        [DataMember(Name = "device_tokens")]
        public List<string> device_tokens { get; set; }

        [DataMember(Name = "aps")]
        public AirshipBody aps { get; set; }

        [DataContract(Name = "apsBody")]
        public class AirshipBody
        {
            [DataMember(Name="alert")]
            public string Alert { get; set; }
        }


    }
}