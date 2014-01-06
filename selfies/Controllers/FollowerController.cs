using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class FollowerController : ApiController
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

        // without parameters, return all followers/contacts from the current user
        public List<follower> Get()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            List<follower> followers = (from follower m in db.followers where m.followerHandle.id.Equals(logged_in.id) select m).ToList();

            return followers;
        }

        [HttpPost]
        public RODResponseMessage Post([FromBody]string s)
        {
            RODResponseMessage result = new RODResponseMessage();

            string user_id = User.Identity.Name;
            handle from_handle = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            handle to_handle = (from m in db.handles where m.name == s && m.active == 1 select m).FirstOrDefault();
            if (to_handle == null)
            {
                result.result = 0;
                result.message = "Handle not found.";
            }
            else
            {

                follower f = new follower();
                f.followeeId = to_handle.id;
                f.followerId = from_handle.id;
                f.active = 1;

                db.followers.Add(f);
                db.SaveChanges();

                result.message = "Contact added.";
                result.result = 1;
            }

            return result;
        }


    }
}
