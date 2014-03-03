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

            List<message> thread_chats = (from message m in db.messages where (m.thread.id.Equals(selected_thread.id) && (m.thread.fromHandleId == logged_in.id
                                      || m.thread.toHandleId == logged_in.id || m.thread.toHandleId == 1)) select m).ToList();


            List<handle> convos = new List<handle>();

            if (thread_chats.Count == 0)
            {
                ViewBag.toKey = logged_in.publicKey;
                convos.Add(selected_thread.toHandle);
            }
            else
            {

                List<int> foundHandleIds = (from m in db.messages where m.threadId.Equals(selected_thread.id) select m.fromHandleId).Distinct().ToList();

                foreach (int x in foundHandleIds)
                {
                    handle foundHandle = (from m in db.handles where m.id == x select m).FirstOrDefault();
                    // don't add the logged in user's id, this is implied
                    if (foundHandle.id != logged_in.id)
                    {
                        convos.Add(foundHandle);
                    }
                }

                if (convos.Count == 0)
                {
                    convos.Add(selected_thread.toHandle);
                }

            }

            ViewBag.convos = convos;

            ViewBag.chats = thread_chats;

            return View();
        }

        public ActionResult Admin()
        {
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            if (logged_in.name != "seth")
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        // profile
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

            DateTime cutoff = DateTime.Today.AddDays(-1);


            List<thread> public_threads = (from thread m in db.threads where
                                               m.fromHandleId.Equals(selected_handle.id) &&
                                               m.toHandleId.Equals(1) &&
                                               m.startDate >= cutoff &&
                                               m.active.Equals(1) select m).ToList();

            public_threads.Reverse();

            ViewData.Model = public_threads;

            return View();
        }

        // inbox
        public ActionResult Index()
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            ViewBag.handle = logged_in;

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

        // this view is shown in-app 
        public ActionResult AppAbout()
        {
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
