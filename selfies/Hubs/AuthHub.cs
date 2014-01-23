using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using selfies.Models;

namespace selfies.Hubs
{

    public class AuthHub : Hub
    {

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
            handle chatter = (handle)handles[connect_id];

            if (chatter == null)
            {
                // pull from database
                // insert into handles
                string user_id = Context.User.Identity.Name;
                chatter = (from handle r in db.handles where r.userGuid.Equals(user_id) select r).FirstOrDefault();
                handles.Add(connect_id, chatter);

                // add to group for easier sending, plus multi-devicers
                Groups.Add(connect_id, chatter.publicKey);
            }

        }

        public void Send(string name, string message, string groupKey)
        {

            string connect_id = Context.ConnectionId;
            handle chatter = (handle)handles[connect_id];
            if (chatter == null)
            {
                // pull from database
                // insert into handles
                string user_id = Context.User.Identity.Name;
                chatter = (from handle r in db.handles where r.userGuid.Equals(user_id) select r).FirstOrDefault();
                handles.Add(connect_id, chatter);

                Groups.Add(connect_id, chatter.publicKey);

            }

            // now loop through all of the clients,
            // and only send the message to the appropriate one...
            thread selected_thread = (from thread m in db.threads where m.groupKey == groupKey select m).FirstOrDefault();

            // how to determine who this should be sent to:
            // -- find the thread with the group key
            // -- check to see if fromHandle is same as chatter,
                  // if it is, then we are to the other person

            string notify_public_key = "";
            if (selected_thread.fromHandle.id == chatter.id)
            {
                notify_public_key = selected_thread.toHandle.publicKey;
            }
            else
            {
                notify_public_key = selected_thread.fromHandle.publicKey;
            }

            Clients.Group(notify_public_key).addMessage(name, message, groupKey);

            message clean_message = new message();
            clean_message.fromHandleId = chatter.id;
            clean_message.sentDate = DateTime.UtcNow;
            clean_message.messageText = message;
            clean_message.threadId = selected_thread.id;

            db.messages.Add(clean_message);
            db.SaveChanges();

        }
    }
}