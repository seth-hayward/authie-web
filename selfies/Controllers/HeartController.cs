using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using selfies.Models;

namespace selfies.Controllers
{
    public class HeartController : ApiController
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


        // GET api/values
        public int Get(string id)
        {

            // get the thread
            // store all the votes in the cache...
            // maybe update when they roll out of the cache
            thread selected_thread = (from thread m in db.threads where m.groupKey.StartsWith(id) select m).FirstOrDefault();
            int hearts = 1;

            if (int.TryParse(selected_thread.hearts.ToString(), out hearts) == false)
            {
                hearts = 1;
            };

            return hearts;

        }

        [HttpPost]
        public int Post(string id)
        {

            thread selected_thread = (from thread m in db.threads where m.groupKey.StartsWith(id) select m).FirstOrDefault();
            int hearts = 1;

            if (int.TryParse(selected_thread.hearts.ToString(), out hearts) == false)
            {
                hearts = 1;
            };

            hearts++;

            selected_thread.hearts = hearts;

            db.threads.Attach(selected_thread);
            var updated_thread = db.Entry(selected_thread);

            updated_thread.Property(e => e.hearts).IsModified = true;
            db.SaveChanges();

            return hearts;

        }

    }
}
