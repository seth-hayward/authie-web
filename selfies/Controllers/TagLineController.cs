using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class TagLineController : ApiController
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

        // GET api/tagline
        // get current user tagline
        public handle Get()
        {
            handle tagline = new handle();
            tagline.id = -1;

            if (User.Identity.IsAuthenticated)
            {
                string user_id = User.Identity.Name;
                tagline = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();                
            }

            return tagline;
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            logged_in.tagLine = value;

            db.handles.Attach(logged_in);
            var updated_handle = db.Entry(logged_in);

            updated_handle.Property(e => e.tagLine).IsModified = true;

            db.SaveChanges();
        }

    }
}
