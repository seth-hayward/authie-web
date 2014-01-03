using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using selfies.Models;

namespace selfies.Controllers
{
    public class HomeController : Controller
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


        public ActionResult Index()
        {
            string user_id = User.Identity.Name;

            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            if (logged_in != null)
            {
                ViewBag.handle = "hey, " + logged_in.name;
            }

            List<thread> threads = (from thread m in db.threads where m.toHandleId.Equals(user_id) || m.fromHandleId.Equals(user_id) select m).ToList();
            ViewBag.threads = threads;

            return View();
        }

        public ActionResult StartThread()
        {

            return View();
        }

    }
}
