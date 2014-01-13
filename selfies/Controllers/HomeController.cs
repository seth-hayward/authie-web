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

            Boolean addEdit = false;
            if (logged_in != null)
            {

                if (logged_in.publicKey == selected_handle.publicKey)
                {

                    addEdit = true;
                    // own profile
                    if (logged_in.tagLine == null)
                    {
                        logged_in.tagLine = "click to add a tagline";
                    }
                }

            }

            ViewBag.addEdit = addEdit;

            List<thread> public_threads = (from thread m in db.threads where m.fromHandleId.Equals(selected_handle.id) && m.toHandleId.Equals(1) && m.active.Equals(1) select m).ToList();

            public_threads.Reverse();

            ViewData.Model = public_threads;

            return View();
        }

        public ActionResult Index()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;


            if (logged_in != null)
            {
                List<thread> threads = (from thread m in db.threads where m.active.Equals(1) && (m.toHandleId.Equals(logged_in.id) || m.fromHandleId.Equals(logged_in.id)) select m).ToList();
                threads.Reverse();
                ViewBag.threads = threads;
            }

            return View();
        }

        public ActionResult Send()
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

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult PrivateKey()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            ViewBag.privateKey = logged_in.privateKey.Substring(0, 5);

            return View();
        }

        public ActionResult Invite()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            return View();
        }

        public ActionResult Daily()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            return View();
        }

        public ActionResult Contacts()
        {

            if (User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("Index");
            }
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

            return View();
        }

    }
}
