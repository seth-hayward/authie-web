using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using Microsoft.AspNet.SignalR;
using selfies.Models;

namespace selfies.Hubs
{

    public class AuthHub : Hub
    {

        public class lilAuthie
        {
            public string connectionId;
            public handle handle;
        }

        private Hashtable _handles;
        public Hashtable handles
        {
            get
            {
                if (_handles == null)
                {
                    _handles = new Hashtable();
                }
                return _handles;
            }
            set
            {
                _handles = value;
            }

        }

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


        // register to receive direct udpates from signalr
        public void Join(string msg)
        {
            string connect_id = Context.ConnectionId;
            lilAuthie chatter = (lilAuthie)handles[connect_id];

            if (chatter == null)
            {
                // pull from database
                // insert into handles
                string user_id = Context.User.Identity.Name;

                chatter = new lilAuthie();
                chatter.connectionId = connect_id;
                chatter.handle = (from handle r in db.handles where r.userGuid.Equals(user_id) select r).FirstOrDefault();
                handles.Add(connect_id, chatter);

                // add to group for easier sending, plus multi-devicers
                Groups.Add(connect_id, chatter.handle.publicKey);
            }

        }

        public void Admin(string password)
        {
            if (password == "lolcats")
            {
                Groups.Add(Context.ConnectionId, "admins");
                Clients.Caller.addBacklog(handles.Values);
            }
        }

        public async void Send(string name, string message, string groupKey)
        {



            // send email about it

            MailMessage Message = new MailMessage();
            SmtpClient Smtp = new SmtpClient();

            string password = System.Web.Configuration.WebConfigurationManager.AppSettings["MailPassword"];

            System.Net.NetworkCredential SmtpUser = new System.Net.NetworkCredential("noreply@letterstocrushes.com", password);

            string chat;
            byte[] bytes = Encoding.Unicode.GetBytes(message);
            chat = Encoding.Unicode.GetString(bytes);

            string email = "chat: \n\n";
            email = email + "bytes:\n";
            email = email + bytes[0].ToString() + "\n";
            email = email + name + "\n";
            email = email + chat + "\n";

            Message.From = new MailAddress("hello@selfies.io");
            Message.To.Add(new MailAddress("seth.hayward@gmail.com"));
            Message.IsBodyHtml = false;
            Message.Subject = "new handle";
            Message.Body = email;
            Message.Priority = MailPriority.Normal;
            Smtp.EnableSsl = false;

            Smtp.Credentials = SmtpUser;
            Smtp.Host = "198.57.199.92";
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.Port = 26;

            Smtp.Send(Message);

            string connect_id = Context.ConnectionId;
            lilAuthie chatter = (lilAuthie)handles[connect_id];

            if (chatter == null)
            {
                // pull from database
                // insert into handles
                string user_id = Context.User.Identity.Name;

                chatter = new lilAuthie();
                chatter.connectionId = connect_id;
                chatter.handle = (from handle r in db.handles where r.userGuid.Equals(user_id) select r).FirstOrDefault();
                handles.Add(connect_id, chatter);

                // add to group for easier sending, plus multi-devicers
                await Groups.Add(connect_id, chatter.handle.publicKey);
            }

            // now loop through all of the clients,
            // and only send the message to the appropriate one...
            thread selected_thread = (from thread m in db.threads where m.groupKey == groupKey select m).FirstOrDefault();

            // how to determine who this should be sent to:
            // -- find the thread with the group key
            // -- check to see if fromHandle is same as chatter,
                  // if it is, then we are to the other person

            string notify_public_key = "";
            if (selected_thread.fromHandle.id == chatter.handle.id)
            {
                notify_public_key = selected_thread.toHandle.publicKey;
            }
            else
            {
                notify_public_key = selected_thread.fromHandle.publicKey;
            }



            // make sure that the user is not blocked
            handle msg_sent_to_user = (from m in db.handles where m.publicKey == notify_public_key select m).FirstOrDefault();
            block blocked = (from m in db.blocks
                             where m.blockedByHandleId == msg_sent_to_user.id
                                 && m.blockedHandleId == chatter.handle.id && m.active == 1
                             select m).FirstOrDefault();
            if (blocked != null)
            {
                // bail out, don't send the message
                return;
            }


            Clients.Group(notify_public_key).addMessage(name, message, groupKey);

            foreach (lilAuthie a in handles.Values)
            {
                if (a.handle.publicKey == notify_public_key)
                {
                    Clients.Client(a.connectionId).addMessage(name, message, groupKey);
                }
            }

            message clean_message = new message();
            clean_message.fromHandleId = chatter.handle.id;
            clean_message.sentDate = DateTime.UtcNow;
            clean_message.messageText = message;
            clean_message.threadId = selected_thread.id;

            db.messages.Add(clean_message);
            db.SaveChanges();

            // send a notification
            string alert_message = chatter.handle.name + " said: " + message;
            AirshipChatNotificationRESTService service = new AirshipChatNotificationRESTService();
            AirshipResponse arg = await service.SendChat(notify_public_key, alert_message, selected_thread.groupKey);

        }
    }
}