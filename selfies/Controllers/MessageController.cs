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


        public MessageController()
        {
            Mapper.CreateMap<message, messageDTO>();
        }

        // return the messages from threads that user can see...
        public List<messageDTO> Get()
        {

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            
            //List<message> msgs = (from m in db.messages where (m.thread.fromHandleId == logged_in.id
            //                      || m.thread.toHandleId == logged_in.id) orderby m.sentDate descending select m).Take(50).ToList();

            DateTime after = DateTime.UtcNow.AddDays(-1);

            // show me the latest messages that are simply to me...
            List<message> msgs = (from m in db.messages
                                  where (m.toHandleId == logged_in.id)
                                  && m.sentDate > after
                                  orderby m.sentDate descending
                                  select m).Take(10).ToList();

            msgs.Reverse();

            List<messageDTO> converted = Mapper.Map<List<message>, List<messageDTO>>(msgs);

            foreach (messageDTO msg in converted)
            {
                string s = msg.thread.groupKey;
                msg.thread = new thread();
                msg.thread.groupKey = s;
                msg.toKey = msg.toHandle.publicKey;
            }

            return converted;
        }

        // return the messages from that thread...
        public List<messageDTO> Get(string id)
        {

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();


            List<message> msgs = new List<message>();
            thread selected_thread = (from thread r in db.threads where r.groupKey == id select r).FirstOrDefault();

            if (selected_thread.fromHandle.id == logged_in.id)
            {
                // return all chats from this groupKey
                msgs = (from m in db.messages
                        where (m.thread.groupKey == id)
                        select m).ToList();
            }
            else
            {
                // toKey is either logged in user
                // or to the dash
                msgs = (from m in db.messages
                        where (m.thread.groupKey == id && (m.toHandle.id == logged_in.id
                            || m.fromHandle.id == logged_in.id || m.toHandle.id == 1))
                        select m).ToList();
            }

                

            List<messageDTO> converted = Mapper.Map<List<message>, List<messageDTO>>(msgs);

            foreach (messageDTO msg in converted)
            {
                string s = msg.thread.groupKey;
                msg.thread = new thread();
                msg.thread.groupKey = s;
                msg.toKey = msg.toHandle.publicKey;
            }

            return converted;
        }

        public async Task<RODResponseMessage> Post(Chat msg)
        {
            RODResponseMessage response = new RODResponseMessage();

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            // now loop through all of the clients,
            // and only send the message to the appropriate one...
            thread selected_thread = (from thread m in db.threads where m.groupKey == msg.groupKey select m).FirstOrDefault();

            // how to determine who this should be sent to:
            // -- find the thread with the group key
            // -- check to see if fromHandle is same as chatter,
                  // if it is, then we are to the other person

            string notify_public_key = "";
            if (selected_thread.fromHandle.id == logged_in.id)
            {
                notify_public_key = selected_thread.toHandle.publicKey;
            }
            else
            {
                notify_public_key = selected_thread.fromHandle.publicKey;
            }


            // toId refers to the opposite of the thread starter.
            // the thread starter can have several convos open,
            // and toId is how we distinguish which one is which.


            //int toId = 1;
            //if (chatter.handle.id == selected_thread.fromHandle.id)
            //{
            //    // chat is from the user who started the thread,
            //    // so how can i get the other person's id...

            //    // it's either the toHandleId...
            //    toId = selected_thread.toHandle.id;
            //}
            //else
            //{
            //    // chat is from user who did not start the thread,
            //    // so the toId is their chatter.handle.id,
            //    toId = chatter.handle.id;
            //}

            handle toHandle = (from m in db.handles where m.publicKey == msg.toKey select m).FirstOrDefault();


            // make sure that the user i not blocked
            handle msg_sent_to_user = (from m in db.handles where m.publicKey == notify_public_key select m).FirstOrDefault();
            block blocked = (from m in db.blocks
                             where m.blockedByHandleId == msg_sent_to_user.id
                                 && m.blockedHandleId == logged_in.id && m.active == 1
                             select m).FirstOrDefault();
            if (blocked != null)
            {
                // bail out, don't send the message
                return new RODResponseMessage() { message = "Blocked.", result = 0 };
            }

            // i really should get a reference to the hub from here,
            // and use that to send out the push message... but since the hub
            // isn't even working yet, we can put that off for now
            // Clients.Client(a.connectionId).addMessage(name, message, groupKey);

            message clean_message = new message();
            clean_message.fromHandleId = logged_in.id;
            clean_message.sentDate = DateTime.UtcNow;
            clean_message.messageText = msg.message;
            clean_message.threadId = selected_thread.id;
            clean_message.toHandleId = toHandle.id;

            db.messages.Add(clean_message);
            db.SaveChanges();

            // send a notification
            string alert_message = logged_in.name + " said, '" + msg.message + "'";
            AirshipChatNotificationRESTService service = new AirshipChatNotificationRESTService();
            AirshipResponse arg = await service.SendChat(notify_public_key, alert_message, selected_thread.groupKey, logged_in.publicKey, clean_message.id);

            response = new RODResponseMessage() { message = "Success.", result = clean_message.id };

            return response;
        }



    }
}
