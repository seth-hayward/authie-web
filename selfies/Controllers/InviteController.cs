using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class InviteController : ApiController
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
            return new RODResponseMessage { result = 0, message = "Not implemented yet" };
        }

        public RODResponseMessage Post(Invite invite)
        {
            if (invite.email != null)
            {

                string user_id = User.Identity.Name;
                handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();

                SendInvite(invite.message, invite.email, logged_in.name);
                // this could be abused right here,
                // it should only send the logged in user's handle, nothing from the client
                return new RODResponseMessage { result = 1, message = "Invite sent. Thanks!" };
            }
            else
            {
                return new RODResponseMessage { result = 0, message = "Please enter an email address." };
            }
        }

        public void SendInvite(string message, string email_address, string handle)
        {
            MailMessage Message = new MailMessage();
            SmtpClient Smtp = new SmtpClient();

            string password = System.Web.Configuration.WebConfigurationManager.AppSettings["MailPassword"];

            System.Net.NetworkCredential SmtpUser = new System.Net.NetworkCredential("noreply@letterstocrushes.com", password);

            string email = "hey, \n\n";
            email = email + "you have been invited to join authie! \n\n";
            email = email + "someone with " + 
                "the handle '" + handle + "' thought you would like it -- so " + 
                "check us out, maybe? \n";
            email = email + "http://authie.me \n\n";
            email = email + "they included this message: \n";
            email = email + message + "\n\n";
            email = email + "<3, the authie team";

            Message.From = new MailAddress("hello@selfies.io");
            Message.To.Add(new MailAddress(email_address));
            Message.IsBodyHtml = false;
            Message.Subject = "Check out authie";
            Message.Body = email;
            Message.Priority = MailPriority.Normal;
            Smtp.EnableSsl = false;

            Smtp.Credentials = SmtpUser;
            Smtp.Host = "198.57.199.92";
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.Port = 26;

            Smtp.Send(Message);
        }

    }
}
