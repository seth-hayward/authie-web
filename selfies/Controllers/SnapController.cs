using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace selfies.Controllers
{
    public class SnapController : ApiController
    {

        public HttpResponseMessage Get(string id, string type)
        {
            string root = HttpContext.Current.Server.MapPath("~/snaps/" + type + "/");
            var path = root + id;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            if (File.Exists(path) == false)
            {
                return new HttpResponseMessage
                ();
            }

            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                 new MediaTypeHeaderValue("image/jpeg");            
            result.StatusCode = HttpStatusCode.OK;

            return result;
        }

    }
}
