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

        //public List<string> device_tokens { get; set; }
        //public AirshipBody aps { get; set; }
        //public class AirshipBody
        //{
        //    public string Alert { get; set; }
        //}

        public AirshipAudience audience { get; set; }
        public class AirshipAudience
        {
            public string alias { get; set; }
        }

        public AirshipNotificationPayload notification { get; set; }
        public class AirshipNotificationPayload
        {
            public string alert { get; set; }
            public AirshipIos ios { get; set; }
            public class AirshipIos
            {
                public AirshipThreadKey extra { get; set; }
                public class AirshipThreadKey
                {
                    public string threadKey { get; set; }
                    public string fromKey { get; set; }
                }
            }
        }

        public List<string> device_types { get; set; }

    }
}