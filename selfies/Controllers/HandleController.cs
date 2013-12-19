using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;
using System.Web.Security;

namespace selfies.Controllers
{
    public class HandleController : ApiController
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


        // GET api/values
        public IEnumerable<handle> Get()
        {
            return (from m in db.handles select m).ToList();
        }

        // GET api/values/5
        public handle Get(int id)
        {
            handle selected = (from m in db.handles where m.id.Equals(id) select m).FirstOrDefault();
            return selected;
        }

        // POST api/values
        public string Post(handle value)
        {

            if (value != null)
            {
                handle currently_exists = (from m in db.handles where m.name.Equals(value.name) select m).FirstOrDefault();

                if (currently_exists == null)
                {

                    handle safe_handle = new handle();
                    Guid public_key = Guid.NewGuid();
                    Guid private_key = Guid.NewGuid();

                    // Attempt to register the user
                    MembershipCreateStatus createStatus;
                    Membership.CreateUser(public_key.ToString(), private_key.ToString(), "anon", null, null, true, null, out createStatus);

                    if (createStatus == MembershipCreateStatus.Success)
                    {
                        FormsAuthentication.SetAuthCookie(public_key.ToString(), true /* createPersistentCookie */);
                        safe_handle.name = value.name;
                        safe_handle.userGuid = public_key.ToString();
                        safe_handle.active = 1;
                        // free handle yay
                        db.handles.Add(safe_handle);
                        db.SaveChanges();
                    }

                }

            }

            return "lol";
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

    }
}
