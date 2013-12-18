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
            ViewBag.selfiesCount = db.selfies.Count();
            ViewBag.handlesCount = db.handles.Count();
            ViewBag.handles = db.handles.ToList();
            return View();
        }
    }
}
