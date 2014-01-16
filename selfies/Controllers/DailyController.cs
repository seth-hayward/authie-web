using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class DailyController : ApiController
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

        // the daily
        public List<thread> Get()
        {

            DateTime cutoff = DateTime.Today.AddDays(-1);

            List<thread> threads = (from thread m in db.threads
                                    where (m.toHandle.publicKey.Equals("2") &&
                                        m.startDate >= cutoff &&
                                        m.active.Equals(1))
                                    select m).ToList();

            threads.Reverse();

            return threads;
        }

    }
}
