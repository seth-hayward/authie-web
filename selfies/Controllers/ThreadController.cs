using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using selfies.Models;
using AutoMapper;

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

        public ThreadController()
        {
            Mapper.CreateMap<thread, threadDTO>();
        }

        // inbox 
        // super secret
        // paginated
        public List<threadDTO> Get(int id = 1)
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();


            if (logged_in == null)
            {
                return new List<threadDTO>();
            }

            List<follower> followers = (from follower m in db.followers
                                      where m.followeeId == logged_in.id &&
                                      m.active == 1
                                      select m).ToList();

            List<int> follower_ids = (from m in followers where m.active == 1 select m.followerHandle.id).ToList();

            // ok, todo
            // show me a list of threads
            // sorted by...
            // MOST RECENT MESSGES at the top
            // -- but we should be able to see those most recent messages!!!!
            // -- will need two different strats for a thread FROM me,
            // -- and a convo that i am just in?
            // ONLY SHOW DISTINCT BY THREAD...
            // so any associated thread where the message.toKey = current user

            //List<message> msgs = (from m in db.messages
            //                      where (m.thread.fromHandleId == logged_in.id
            //                          || m.thread.toHandleId == logged_in.id)
            //                      orderby m.sentDate descending
            //                      select m).Take(50).ToList();

            //List<thread> threads = (from m in msgs select m.thread).Distinct().ToList();

            //msgs.Reverse();

            List<thread> threads = (from thread m in db.threads
                                    orderby m.startDate descending
                                    where
                                        (m.toHandle.id == logged_in.id ||
                                        m.fromHandleId == logged_in.id ||
                                        (follower_ids.Contains(m.fromHandleId) && (m.toHandle.id == logged_in.id || m.toHandle.id == 1))
                                        )
                                        && m.active == 1
                                    select m).Distinct().ToList();

            //List<thread> threads = (from thread m in db.threads
            //                        where
            //                            (m.toHandleId.Equals(logged_in.id)
            //                            || m.fromHandleId.Equals(logged_in.id)
            //                            ) &
            //                            m.active.Equals(1)
            //                        select m).ToList();

            //List<thread> follower_threads = (from thread m in db.threads
            //                                 where
            //                                     follower_ids.Contains(m.fromHandleId)
            //                                     && m.toHandleId == 1 && m.active == 1
            //                                 select m).ToList();

            //List<thread> ordered_threads = threads.Union(follower_threads).ToList();

            List<threadDTO> computed_threads = Mapper.Map<List<thread>, List<threadDTO>>(threads);

            foreach (threadDTO d in computed_threads)
            {

                List<message> msg = (from m in db.messages where m.threadId.Equals(d.id)
                                     orderby m.sentDate descending
                                     select m).ToList();

                if (msg.Count == 0)
                {
                    d.lastMessageDate = (DateTime)d.startDate;
                }
                else
                {
                    d.lastMessageDate = msg.First().sentDate;
                }

                // only need to do this if we 
                // are looking at a thread that is
                // from the logged_in user...
                d.convos = new List<handle>();

                if (d.fromHandleId == logged_in.id)
                {

                    List<int> foundHandleIds = (from m in db.messages where m.threadId.Equals(d.id) select m.fromHandleId).Distinct().ToList();

                    foreach (int x in foundHandleIds)
                    {
                        handle foundHandle = (from m in db.handles where m.id == x select m).FirstOrDefault();

                        // don't add the logged in user's id, this is implied

                        if (foundHandle.id != logged_in.id)
                        {
                            d.convos.Add(foundHandle);
                        }
                    }

                }
                else
                {
                    // you should only see the from handle then??
                    d.convos.Add(d.fromHandle);
                }

            }

            computed_threads = (from m in computed_threads orderby m.lastMessageDate descending select m).ToList();

            computed_threads = computed_threads.Skip((id - 1) * 10).Take(10).ToList();

            return computed_threads;
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

        public async Task<RODResponseMessage> Post(Snap _snap)
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


            // check to make sure that the thread sender 
            // is not blocked by the recipent
            block blocked = (from m in db.blocks
                             where m.blockedByHandleId == to_handle.id
                                 && m.blockedHandleId == logged_in.id && m.active == 1
                             select m).FirstOrDefault();
            if (blocked != null)
            {
                // bail out, don't send the message
                msg.result = -1;
                msg.message = "Unable to send message.";
                return msg;
            }


            thread clean_thread = new thread();
            clean_thread.active = 1;
            clean_thread.startDate = DateTime.UtcNow;
            clean_thread.fromHandleId = logged_in.id;
            clean_thread.caption = _snap.caption;
            clean_thread.hearts = 0;
            clean_thread.toHandleId = to_handle.id;
            clean_thread.location = _snap.location;
            clean_thread.font = _snap.font;
            clean_thread.textColor = _snap.textColor;

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

                if (_snap.caption.Length > 0)
                {
                    message chat_message = new message();
                    chat_message.threadId = clean_thread.id;
                    chat_message.messageText = clean_thread.caption;
                    chat_message.fromHandleId = clean_thread.fromHandleId;
                    chat_message.sentDate = DateTime.UtcNow;
                    chat_message.toHandleId = to_handle.id;
                    db.messages.Add(chat_message);
                    db.SaveChanges();
                }
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
