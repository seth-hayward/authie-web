using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class MessageController : ApiController
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

        // return the messages from threads that user can see...
        public List<message> Get()
        {

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            
            List<message> msgs = (from m in db.messages where (m.thread.fromHandleId == logged_in.id
                                  || m.thread.toHandleId == logged_in.id) select m).ToList();
            return msgs;
        }

        // return the messages from that thread...
        public List<message> Get(string groupKey)
        {
            List<message> msgs = (from m in db.messages where m.thread.groupKey == groupKey select m).ToList();
            return msgs;
        }

        public RODResponseMessage Post(message msg)
        {
            RODResponseMessage response = new RODResponseMessage();


            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            message clean_message = new message();
            clean_message.fromHandleId = logged_in.id;
            clean_message.sentDate = DateTime.UtcNow;

            string groupKey = msg.thread.groupKey;
            thread referring_thread = (from thread r in db.threads where r.groupKey == groupKey).FirstOrDefault();

            if(referring_thread == null) {
                response.result = 0;
                response.message = "Null referring thread";
            } else {
                clean_message.threadId = referring_thread.id;
                response.result = 1;
                response.message = "Success";

                db.messages.Add(clean_message);
                db.SaveChanges();

            }

            return response;
        }

    }
}
