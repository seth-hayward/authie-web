using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;
using AutoMapper;

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

        public FollowerController()
        {
            Mapper.CreateMap<follower, followerDTO>();
        }

        // without parameters, return all followers/contacts from the current user
        public List<followerDTO> Get()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            // fake the dash

            follower dash = new follower();

            dash.active = 1;
            dash.followeeHandle = new handle();
            dash.followeeHandle.name = "dash";
            dash.followeeHandle.publicKey = "1";

            // fake the wire

            //follower wire = new follower();

            //wire.active = 1;
            //wire.followeeHandle = new handle();
            //wire.followeeHandle.name = "the wire";
            //wire.followeeHandle.publicKey = "2";

            List<follower> followers = (from follower m in db.followers where m.followerHandle.id.Equals(logged_in.id) && m.active.Equals(1) select m).ToList();

            //followers.Insert(0, wire);
            followers.Insert(0, dash);

            List<followerDTO> followersDTO = Mapper.Map<List<follower>, List<followerDTO>>(followers);


            foreach (followerDTO d in followersDTO)
            {

                if(d.followeeHandle.name == "dash") {

                    thread yourMostRecentDash = (from m in db.threads
                                                 where
                                                     m.fromHandleId == logged_in.id &&
                                                     m.active == 1 && m.toHandleId == 1
                                                 orderby m.id descending
                                                 select m).FirstOrDefault();

                    if (yourMostRecentDash != null)
                    {
                        d.mostRecentSnap = yourMostRecentDash.groupKey;
                    }

                } else {

                    thread t = (from m in db.threads
                                where m.fromHandleId == d.followeeId
                                    && m.active == 1 && m.toHandleId == 1
                                orderby m.id descending
                                select m).FirstOrDefault();

                    if (t != null)
                    {
                        d.mostRecentSnap = t.groupKey;
                    }


                }
            }

            return followersDTO;
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

                // check to make sure that this person is not blocked

                block blocked = (from m in db.blocks where m.blockedByHandleId == to_handle.id
                                 && m.blockedHandleId == from_handle.id && m.active == 1 select m).FirstOrDefault();

                if (blocked != null)
                {
                    result.message = "Unable to send request.";
                    result.result = 0;
                    return result;
                }



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

            int removeResult = removeContact(to_handle.id);

            if (removeResult == 0)
            {
                result.result = 0;
                result.message = "Contact not found.";
            }
            else
            {
                result.message = "Contact removed.";
                result.result = 1;
            }

            return result;
        }

        public int removeContact(int remove_id)
        {

            handle to_handle = (from m in db.handles where m.id == remove_id && m.active == 1 select m).FirstOrDefault();
            handle from_handle = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            follower follow_connect = (from m in db.followers where m.followerHandle.id == from_handle.id && m.followeeHandle.id == to_handle.id && m.active == 1 select m).FirstOrDefault();
            follower followee_connect = (from m in db.followers where m.followerHandle.id == to_handle.id && m.followeeHandle.id == from_handle.id && m.active == 1 select m).FirstOrDefault();

            if (follow_connect == null || followee_connect == null)
            {
                return 0;
            }
            else
            {
                follow_connect.active = 0;

                db.followers.Attach(follow_connect);

                var updated_follower = db.Entry(follow_connect);
                updated_follower.Property(e => e.active).IsModified = true;

                db.SaveChanges();

                followee_connect.active = 0;

                db.followers.Attach(followee_connect);

                var updated_followee = db.Entry(followee_connect);
                updated_followee.Property(e => e.active).IsModified = true;

                db.SaveChanges();

                return 1;
            }

        }


    }
}
