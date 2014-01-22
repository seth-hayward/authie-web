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

            DateTime cutoff = DateTime.Today.AddDays(-1);

            List<thread> threads = (from thread m in db.threads
                                    where (m.fromHandle.publicKey.Equals(publicKey) &&
                                        m.startDate >= cutoff &&
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
            clean_thread.hearts = 1;
            clean_thread.toHandleId = to_handle.id;

            //
            // check to see if the to_handle is authorized
            // to make the send, if they are not, then this is
            // an authorization request. 
            //
            clean_thread.authorizeRequest = 0;

            //
            // we don't need to do this check if this is for
            // toHandleId = 1 or toHandleId = 2 (snaps sent
            // to profile and the daily)
            //

            if (clean_thread.toHandleId != 1 && clean_thread.toHandleId != 2)
            {

                follower confirmed_to_handle_follower = (from m in db.followers
                                                         where m.followeeId == to_handle.id &&
                                                         m.followerId == logged_in.id &&
                                                         m.active == 1
                                                         select m).FirstOrDefault();

                if (confirmed_to_handle_follower == null)
                {
                    clean_thread.authorizeRequest = 1;
                }

            }

            // should it all be organized around this group key then?
            // add new threads and messages at the same with the same
            // group key generated on the client (like a guid)...

            clean_thread.groupKey = _snap.groupKey;
            db.threads.Add(clean_thread);
            db.SaveChanges();

            if (_snap.caption != null)
            {
                message chat_message = new message();
                chat_message.threadId = clean_thread.id;
                chat_message.messageText = clean_thread.caption;
                chat_message.fromHandleId = clean_thread.fromHandleId;
                db.messages.Add(chat_message);
                db.SaveChanges();
            }

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
