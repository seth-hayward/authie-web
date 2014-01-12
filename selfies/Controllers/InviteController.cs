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


        public RODResponseMessage Get()
        {
            SendInvite("Hey.", "seth.hayward@gmail.com", "seth");
            return new RODResponseMessage { result = 0, message = "Not implemented yet" };
        }

        public void SendInvite(string message, string email_address, string from_handle)
        {
            MailMessage Message = new MailMessage();
            SmtpClient Smtp = new SmtpClient();

            string password = System.Web.Configuration.WebConfigurationManager.AppSettings["MailPassword"];

            System.Net.NetworkCredential SmtpUser = new System.Net.NetworkCredential("hello@selfies.io", password);

            string email = "hey, \n";
            email = email + "you have been invited to join authie. someone with " + 
                "the handle '" + from_handle + "', thought you would like it -- so " + 
                "check us out, maybe? \n";
            email = email + "http://authie.me \n";
            email = email + "\n";
            email = email + "\n";
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
