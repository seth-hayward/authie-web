using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class CheckHandleController : ApiController
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


        // POST api/values
        public RODResponseMessage Post(handle check_handle)
        {

            handle currently_exists = (from m in db.handles where m.name.Equals(check_handle.name) && m.active == 1 select m).FirstOrDefault();

            // always return handle name so the client knows which response
            // they are getting

            if (currently_exists == null)
            {
                return new RODResponseMessage { message = check_handle.name, result = 1 };
            }
            else
            {
                return new RODResponseMessage { message = check_handle.name, result = 0 };
            }


        }

    }
}
