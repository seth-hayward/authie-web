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

            List<handle> handles = (from handle m in db.handles where m.active == 1 select m).ToList();
            ViewBag.handles = handles;

            return View();
        }
    }
}
