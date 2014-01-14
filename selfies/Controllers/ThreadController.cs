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

        // inbox 
        // super secret
        public List<thread> Get()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            List<thread> threads = (from thread m in db.threads where (m.toHandleId.Equals(logged_in.id) || m.fromHandleId.Equals(logged_in.id)) &
                                    m.active.Equals(1) select m).ToList();

            threads.Reverse(); 

            return threads;
        }

        // profile, 24 hour view of all public posts from this key
        public List<thread> Get(string publicKey)
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            List<thread> threads = (from thread m in db.threads
                                    where (m.fromHandle.publicKey.Equals(publicKey) &
                                        m.active.Equals(1) && m.toHandleId.Equals(1))
                                    select m).ToList();

            threads.Reverse();

            return threads;
        }

        public thread Get(int id)
        {
            thread selected = (from m in db.threads where m.id.Equals(id) select m).FirstOrDefault();
            return selected;
        }

        [HttpPost]
        public RODResponseMessage Post(Snap _snap)
        {

            RODResponseMessage msg = new RODResponseMessage();

            if (_snap == null)
            {
                msg.result = -1;
                msg.message = "Snap was null.";
                return msg;
            }

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            handle to_handle = (from handle r in db.handles where r.publicKey == _snap.toGuid select r).FirstOrDefault();

            thread clean_thread = new thread();
            clean_thread.active = 1;
            clean_thread.startDate = DateTime.UtcNow;
            clean_thread.fromHandleId = logged_in.id;
            clean_thread.caption = _snap.caption;
            clean_thread.authorizeRequest = 0;
            clean_thread.hearts = 1;

            // should it all be organized around this group key then?
            // add new threads and messages at the same with the same
            // group key generated on the client (like a guid)...

            clean_thread.groupKey = _snap.groupKey;
            clean_thread.toHandleId = to_handle.id;
            db.threads.Add(clean_thread);
            db.SaveChanges();

            msg.message = clean_thread.id.ToString();
            msg.result = 1;

            return msg;
        }


        [HttpDelete]
        public RODResponseMessage Delete([FromBody]string s)
        {
            RODResponseMessage result = new RODResponseMessage();

            thread selected = (from m in db.threads where m.groupKey.Equals(s) select m).FirstOrDefault();

            string user_id = User.Identity.Name;
            handle from_handle = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            // only thread starter can delete for now
            if (selected.fromHandle.id.Equals(from_handle.id) || selected.toHandle.id.Equals(from_handle.id))
            {

                selected.active = 0;

                db.threads.Attach(selected);
                var updated_thread = db.Entry(selected);

                updated_thread.Property(e => e.active).IsModified = true;
                db.SaveChanges();

                result.result = 1;
                result.message = "Success.";

            }
            else
            {
                result.result = 0;
                result.message = "Unauthorized.";
            }

            return result;

        }
    }
}
