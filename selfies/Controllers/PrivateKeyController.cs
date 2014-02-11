using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class PrivateKeyController : ApiController
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

        public RODResponseMessage Get()
        {

            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

            // always return handle name so the client knows which response
            // they are getting

            CryptoLibrary.AES encrypter = new CryptoLibrary.AES();
            string aes_encrypted_key = encrypter.Encrypt(logged_in.privateKey.Substring(0, 10), "salty");

            if (logged_in.name == null)
            {
                return new RODResponseMessage { message = "error", result = 0 };
            }
            else
            {
                return new RODResponseMessage { message = aes_encrypted_key, result = 1 };
            }

        }
    }
}
