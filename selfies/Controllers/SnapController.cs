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

        public HttpResponseMessage Get(string id)
        {
            string root = HttpContext.Current.Server.MapPath("~/snaps/");
            var path = root + id;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            var stream = new FileStream(path, FileMode.Open);
            result.Content = new StreamContent(stream);
            result.Content.Headers.ContentType =
                new MediaTypeHeaderValue("application/octet-stream");
            return result;
        }

    }
}
