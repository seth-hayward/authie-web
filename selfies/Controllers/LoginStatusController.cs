using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class LoginStatusController : ApiController
    {

        // GET api/values/5
        public RODResponseMessage Get()
        {

            RODResponseMessage msg = new RODResponseMessage();
            if (User.Identity.IsAuthenticated)
            {
                msg.result = 1;
                msg.message = User.Identity.Name;
            }
            else
            {
                msg.result = 0;
                msg.message = "Anon";
            }

            return msg;
        }


    }
}
