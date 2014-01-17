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
    public class ReportController : ApiController
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

        public int Post(thread id)
        {

            thread selected_thread = (from thread m in db.threads where m.groupKey.StartsWith(id.groupKey) select m).FirstOrDefault();

            MailMessage Message = new MailMessage();
            SmtpClient Smtp = new SmtpClient();

            string password = System.Web.Configuration.WebConfigurationManager.AppSettings["MailPassword"];

            System.Net.NetworkCredential SmtpUser = new System.Net.NetworkCredential("noreply@letterstocrushes.com", password);

            string email = "reported: \n\n";
            email = email + "http://authie.me/thread/" + selected_thread.groupKey + "\n\n";

            Message.From = new MailAddress("hello@selfies.io");
            Message.To.Add(new MailAddress("seth.hayward@gmail.com"));
            Message.IsBodyHtml = false;
            Message.Subject = "authie report";
            Message.Body = email;
            Message.Priority = MailPriority.Normal;
            Smtp.EnableSsl = false;

            Smtp.Credentials = SmtpUser;
            Smtp.Host = "198.57.199.92";
            Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            Smtp.Port = 26;

            Smtp.Send(Message);


            return selected_thread.id ;

        }

    }
}
