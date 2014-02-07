using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class BlockController : ApiController
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

        public List<block> Get()
        {

            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            List<block> blocks = (from m in db.blocks
                                  where m.blockedByHandleId == logged_in.id
                                  select m).ToList();

            return blocks;
        }

        public block Post(string id)
        {
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            handle blocked_user = (from m in db.handles where m.publicKey == id select m).FirstOrDefault();

            block b = new block();

            if (blocked_user != null)
            {
                b.active = 1;
                b.blockedByHandleId = logged_in.id;
                b.blockedHandleId = blocked_user.id;
                b.blockedOn = DateTime.UtcNow;
                db.blocks.Add(b);
                db.SaveChanges();
            }
            else
            {
                b.active = 0;
            }

            return b;
        }

    }
}
