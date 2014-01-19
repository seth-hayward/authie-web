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

        // return the messages from that thread...
        public List<message> Get(string groupKey)
        {
            List<message> msgs = (from m in db.messages where m.thread.groupKey == groupKey select m).ToList();
            return msgs;
        }

        public RODResponseMessage Post(message msg)
        {
            RODResponseMessage response = new RODResponseMessage();

            response.result = 0;
            response.message = "Not implemented.";

            return response;
        }

    }
}
