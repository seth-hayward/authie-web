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

            // fake the dash

            follower dash = new follower();

            dash.active = 1;
            dash.followeeHandle = new handle();
            dash.followeeHandle.name = "dash";

            List<follower> followers = (from follower m in db.followers where m.followerHandle.id.Equals(logged_in.id) && m.active.Equals(1) select m).ToList();

            followers.Insert(0, dash);

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

                follower currently_added = (from m in db.followers where m.followerHandle.id.Equals(from_handle.id) && m.followeeHandle.name.Equals(s) && m.active.Equals(1) select m).FirstOrDefault();

                if (currently_added != null)
                {
                    result.message = "Person is already a contact.";
                    result.result = 0;
                    return result;
                }

                // DISABLED --> for now ^^
                //follower f = new follower();
                //f.followeeId = to_handle.id;
                //f.followerId = from_handle.id;
                //f.active = 1;
                //db.followers.Add(f);
                //db.SaveChanges();

                result.message = to_handle.publicKey;
                result.result = 1;
            }

            return result;
        }

        [HttpDelete]
        public RODResponseMessage Delete([FromBody]string s)
        {
            RODResponseMessage result = new RODResponseMessage();

            handle to_handle = (from m in db.handles where m.name == s && m.active == 1 select m).FirstOrDefault();
            handle from_handle = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            follower follow_connect = (from m in db.followers where m.followerHandle.id == from_handle.id && m.followeeHandle.name.Equals(s) && m.active == 1 select m).FirstOrDefault();
            if (follow_connect == null)
            {
                result.result = 0;
                result.message = "Contact not found.";
            }
            else
            {

                follow_connect.active = 0;

                db.followers.Attach(follow_connect);

                var updated_follower = db.Entry(follow_connect);
                updated_follower.Property(e => e.active).IsModified = true;

                db.SaveChanges();

                result.message = "Contact removed.";
                result.result = 1;
            }

            return result;
        }


    }
}
