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
            return new RODResponseMessage { result = 0, message = "Not implemented yet" };
        }

        public void SendContact(string message_body, string email_address)
        {
            MailMessage Message = new MailMessage();
            SmtpClient Smtp = new SmtpClient();
            System.Net.NetworkCredential SmtpUser = new System.Net.NetworkCredential("noreply@letterstocrushes.com", l_mail_password);

            message_body = "<html><head></head><body>" + message_body + "</body></html>";

            Message.From = new MailAddress("noreply@letterstocrushes.com");
            Message.To.Add(new MailAddress("seth.hayward@gmail.com"));
            Message.IsBodyHtml = true;
            Message.Subject = "feedback: " + email_address;
            Message.Body = message_body;
            Message.Priority = MailPriority.Normal;
            Smtp.EnableSsl = false;

            Smtp.Credentials = SmtpUser;
            Smtp.Host = "198.57.199.92";
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.Port = 26;

            List<string> ignore_phrases = new List<string>();
            ignore_phrases.Add("url=http:");

            Smtp.Send(Message);
        }

    }
}
