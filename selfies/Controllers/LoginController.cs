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

            if (login.publicKey.Length <= 4)
            {
                msg.result = 0;
                msg.message = "Unable to login. Please re-type your private key (case sensitive.)";
                return msg;
            }

            handle og = (from m in db.handles
                         where m.name == login.name.ToLower() && m.active == 1 && m.privateKey.StartsWith(login.publicKey)
                         select m).FirstOrDefault();            
            
            if(og != null && Membership.ValidateUser(og.publicKey, og.privateKey)) {
                FormsAuthentication.SetAuthCookie(og.publicKey, true);
                msg.result = 1;
                msg.message = "Welcome.";
            } else {
                msg.result = 0;
                msg.message = "Unable to login. Please re-type your private key (case sensitive.)";
            }

            return msg;
        }

    }
}
