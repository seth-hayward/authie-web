using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class ThreadController : ApiController
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

        // without parameters, return all threads to and from the current user
        public List<thread> Get()
        {
            string user_id = User.Identity.Name;
            List<thread> threads = (from thread m in db.threads where m.toHandleId.Equals(user_id) || m.fromHandleId.Equals(user_id) select m).ToList();

            foreach(thread lx in threads) {
                if (lx.toHandleId == "1")
                {
                    lx.toHandleId = "everyone";
                }
            }
            return threads;
        }

        public thread Get(int id)
        {
            thread selected = (from m in db.threads where m.id.Equals(id) select m).FirstOrDefault();
            return selected;
        }

        public RODResponseMessage Post(thread new_thread)
        {

            RODResponseMessage msg = new RODResponseMessage();

            string user_id = User.Identity.Name;

            if (user_id != new_thread.fromHandleId)
            {
                msg.result = 0;
                msg.message = "fromHandleId doesn't match logged in user.";
                return msg;
            }

            thread clean_thread = new thread();
            clean_thread.startDate = DateTime.UtcNow;
            clean_thread.fromHandleId = User.Identity.Name;

            // should it all be organized around this group key then?
            // add new threads and messages at the same with the same
            // group key generated on the client (like a guid)...

            clean_thread.groupKey = new_thread.groupKey;
            clean_thread.toHandleId = new_thread.toHandleId;
            db.threads.Add(clean_thread);
            db.SaveChanges();

            msg.message = clean_thread.id.ToString();
            msg.result = 1;

            return msg;
        }

    }
}
