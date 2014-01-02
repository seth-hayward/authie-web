using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Security;
using selfies.Models;

namespace selfies.Controllers
{
    public class LoginController : ApiController
    {

        // return login status
        // GET api/login
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

        public RODResponseMessage Post(handle login)
        {

            RODResponseMessage msg = new RODResponseMessage();
            if(Membership.ValidateUser(login.publicKey, login.privateKey)) {
                FormsAuthentication.SetAuthCookie(login.publicKey, true);
                msg.result = 1;
                msg.message = "Welcome.";
            } else {
                msg.result = 0;
                msg.message = "Unable to login.";
            }

            return msg;
        }

    }
}
