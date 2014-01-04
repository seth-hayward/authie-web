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

        public ActionResult Thread(string key)
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            thread selected_thread = (from thread m in db.threads where m.groupKey.StartsWith(key) select m).FirstOrDefault();
            ViewData.Model = selected_thread;

            return View();
        }

        public ActionResult Details(string handle)
        {


            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            handle selected_handle = (from handle r in db.handles where r.name.Equals(handle) && r.active == 1 select r).FirstOrDefault();
            ViewBag.selectedHandle = selected_handle;

            ViewBag.addEdit = false;
            if (logged_in != null)
            {

                if (logged_in.publicKey == selected_handle.publicKey)
                {

                    ViewBag.addEdit = true;
                    // own profile
                    if (logged_in.tagLine == null)
                    {
                        logged_in.tagLine = "click to add a tagline";
                    }
                }

            }


            List<thread> public_threads = (from thread m in db.threads where m.fromHandleId.Equals(selected_handle.publicKey) && m.toHandleId.Equals("1") select m).ToList();
            ViewData.Model = public_threads;

            return View();
        }

        public ActionResult Index()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            List<thread> threads = (from thread m in db.threads where m.toHandleId.Equals(user_id) || m.fromHandleId.Equals(user_id) select m).ToList();
            ViewBag.threads = threads;

            return View();
        }

        public ActionResult StartThread()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            return View();        
        }

        public ActionResult UploadSnap(string guid)
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            ViewBag.guid = guid;

            return View();        

        }

        public ActionResult Register()
        {
            return View();
        }

        public ActionResult What()
        {
            return View();
        }

    }
}
