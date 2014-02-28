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
            
            List<message> msgs = (from m in db.messages where (m.thread.fromHandleId == logged_in.id
                                  || m.thread.toHandleId == logged_in.id) orderby m.sentDate descending select m).Take(50).ToList();

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

            List<message> msgs = (from m in db.messages
                                  where (m.thread.groupKey == id && (m.thread.fromHandleId == logged_in.id
                                      || m.thread.toHandleId == logged_in.id || m.thread.toHandleId == 1))
                                  select m).ToList();

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

        public async Task<RODResponseMessage> Post(message msg)
        {
            RODResponseMessage response = new RODResponseMessage();

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            message clean_message = new message();
            clean_message.fromHandleId = logged_in.id;
            clean_message.sentDate = DateTime.UtcNow;
            clean_message.messageText = msg.messageText;

            string groupKey = msg.thread.groupKey;
            thread referring_thread = (from thread r in db.threads where r.groupKey == groupKey select r).FirstOrDefault();

            // if logged_in = referring_thread.fromHandle,
            // then use toHandle,

            string to_key;

            if(logged_in.id == referring_thread.fromHandle.id) {
                // it's the logged in user's thread, send it 
                // to the other guy
                to_key = referring_thread.toHandle.publicKey;
            } else {
                // it's the other guy's thread, send it to
                // the other guy
                to_key = referring_thread.fromHandle.publicKey;
            }

            if(referring_thread == null) {

                response.result = 0;
                response.message = "Unable to find thread";

            } else {
                clean_message.threadId = referring_thread.id;
                response.result = 1;
                response.message = "Success";

                db.messages.Add(clean_message);
                db.SaveChanges();

                string alert_text = logged_in.name + " said: " + msg.messageText;

                // post the message to urbanairship now
                AirshipChatNotificationRESTService service = new AirshipChatNotificationRESTService();
                AirshipResponse rep = await service.SendChat(to_key, alert_text, referring_thread.groupKey);

            }

            return response;
        }



    }
}
