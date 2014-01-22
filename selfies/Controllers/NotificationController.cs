using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class NotificationController : ApiController
    {

        private selfiesMySQL _db;
        public selfiesMySQL db
        {
            get
            {
                if (_db == null)
                {
                    _db = new selfiesMySQL();
                }
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        public RODResponseMessage Get(thread thready)
        {
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            thread selected_thread = (from thread r in db.threads where r.groupKey == thready.groupKey select r).FirstOrDefault();

            RODResponseMessage msg = new RODResponseMessage();

            msg.result = 0;
            msg.message = "Error.";

            if (selected_thread != null && selected_thread.fromHandle.id == logged_in.id)
            {

                string alert_message = logged_in.name + " sent you a snap";
                // post the message to urbanairship now
                AirshipChatNotificationRESTService service = new AirshipChatNotificationRESTService();
                service.SendChat(selected_thread.toHandle.publicKey, alert_message, thready.groupKey);

                msg.result = 1;
                msg.message = "Success";
            }

            return msg;
        }

    }
}
