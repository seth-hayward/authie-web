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
    public class BlockController : ApiController
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

        public List<block> Get()
        {

            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            List<block> blocks = (from m in db.blocks
                                  where m.blockedByHandleId == logged_in.id
                                  select m).ToList();

            return blocks;
        }

        public block Post(string id)
        {
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            handle blocked_user = (from m in db.handles where m.publicKey == id select m).FirstOrDefault();

            block b = new block();

            if (blocked_user != null)
            {
                b.active = 1;
                b.blockedByHandleId = logged_in.id;
                b.blockedHandleId = blocked_user.id;
                b.blockedOn = DateTime.UtcNow;
                db.blocks.Add(b);

                // remove original contact if it exists
                follower follow_connect = (from m in db.followers where m.followerHandle.id == logged_in.id && m.followeeHandle.id == blocked_user.id && m.active == 1 select m).FirstOrDefault();
                if (follow_connect != null)
                {
                    follow_connect.active = 0;

                    db.followers.Attach(follow_connect);

                    var updated_follower = db.Entry(follow_connect);
                    updated_follower.Property(e => e.active).IsModified = true;
                }

                // remove the other person's contact as well
                follower blocked_connect = (from m in db.followers where m.followerHandle.id == blocked_user.id && m.followeeHandle.id == logged_in.id && m.active == 1 select m).FirstOrDefault();
                if (blocked_connect != null)
                {
                    blocked_connect.active = 0;

                    db.followers.Attach(blocked_connect);

                    var updated_block_follower = db.Entry(blocked_connect);
                    updated_block_follower.Property(e => e.active).IsModified = true;
                }

                db.SaveChanges();

                string password = System.Web.Configuration.WebConfigurationManager.AppSettings["MailPassword"];

                MailMessage Message = new MailMessage();
                SmtpClient Smtp = new SmtpClient();

                System.Net.NetworkCredential SmtpUser = new System.Net.NetworkCredential("noreply@letterstocrushes.com", password);

                string email = logged_in.name + " blocked " + blocked_user.name;

                Message.From = new MailAddress("hello@selfies.io");
                Message.To.Add(new MailAddress("seth.hayward@gmail.com"));
                Message.IsBodyHtml = false;
                Message.Subject = "block";
                Message.Body = email;
                Message.Priority = MailPriority.Normal;
                Smtp.EnableSsl = false;

                Smtp.Credentials = SmtpUser;
                Smtp.Host = "198.57.199.92";
                Smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                Smtp.Port = 26;

                Smtp.Send(Message);



            }
            else
            {
                b.active = 0;
            }

            return b;
        }

        public void removeContact(int remove_id)
        {



        }

    }
}
