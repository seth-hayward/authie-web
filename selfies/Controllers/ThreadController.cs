﻿using System;
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
                if (lx.toHandleId == 1)
                {
                    lx.toHandleId = 1;
                }
            }
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

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            handle to_handle = (from handle r in db.handles where r.publicKey == _snap.toGuid select r).FirstOrDefault();

            thread clean_thread = new thread();
            clean_thread.startDate = DateTime.UtcNow;
            clean_thread.fromHandleId = logged_in.id;

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

        public void Delete(int id)
        {
            thread selected = (from m in db.threads where m.id.Equals(id) select m).FirstOrDefault();

            string user_id = User.Identity.Name;
            handle from_handle = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();




        }
    }
}
