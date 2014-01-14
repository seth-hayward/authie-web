using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class AuthorizeController : ApiController
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

        [HttpPost]
        public RODResponseMessage Post([FromBody]string s)
        {
            RODResponseMessage result = new RODResponseMessage();

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            // the person posting to this method has just had
            // an authorization request sent to THEM, so they need
            // to get the handle of the sender (From handle)

            handle from_handle = (from m in db.handles where m.publicKey == s && m.active == 1 select m).FirstOrDefault();
            if (from_handle == null)
            {
                result.result = 0;
                result.message = "Handle not found.";
            }
            else
            {

                follower added = (from m in db.followers where
                                  m.followerHandle.id.Equals(from_handle.id) &&
                                  m.followeeHandle.publicKey.Equals(s) &&
                                  m.active.Equals(1) select m).FirstOrDefault();

                if (added != null)
                {
                    result.message = "Person is already a contact.";
                    result.result = 0;
                    return result;
                }


                follower f = new follower();
                f.followeeId = from_handle.id;
                f.followerId = logged_in.id;
                f.active = 1;
                db.followers.Add(f);

                // trash the old message

                thread request = (from m in db.threads
                                  where m.toHandleId.Equals(logged_in.id) &&
                                      m.fromHandleId == from_handle.id &&
                                      m.active == 1 &&
                                      m.authorizeRequest == 1
                                  select m).FirstOrDefault();

                if (request != null)
                {
                    // it really has to exist, otherwise something
                    // sketchy is going on
                    request.active = 0;

                    db.threads.Attach(request);
                    var updated_request = db.Entry(request);

                    updated_request.Property(e => e.active).IsModified = true;
                }

                db.SaveChanges();

                result.message = "Successful add.";
                result.result = 1;
            }

            return result;
        }
    }
}
