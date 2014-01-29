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
            string orig = HttpContext.Current.Server.MapPath("~/snaps/orig/");
            string root = HttpContext.Current.Server.MapPath("~/snaps/" + type + "/");
            var path = root + id;
            var orig_path = orig + id;

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            if (File.Exists(path) == false)
            {

                // try regenerating it if the original file exists
                if(File.Exists(orig_path) == true) {

                    FileInfo fileInfo = new FileInfo(orig_path);

                    // generate 640px version (retina display)
                    string sz640_dir = HttpContext.Current.Server.MapPath("~/snaps/640/");
                    string filename = sz640_dir + id;
                    ImageResizer.ImageJob i = new ImageResizer.ImageJob(orig_path, filename, new ImageResizer.Instructions(
                                    "width=640;height=900;format=jpg;mode=max;autorotate=true"));
                    i.CreateParentDirectory = true; //Auto-create the uploads directory.
                    i.Build();

                    // generate a 500px version
                    string sz500_dir = HttpContext.Current.Server.MapPath("~/snaps/500/");
                    filename = sz500_dir + id;
                    i = new ImageResizer.ImageJob(orig_path, filename, new ImageResizer.Instructions(
                                    "width=500;height=900;format=jpg;mode=max;autorotate=true"));
                    i.CreateParentDirectory = true; //Auto-create the uploads directory.
                    i.Build();


                    // generate thumb version
                    string thumb_dir = HttpContext.Current.Server.MapPath("~/snaps/thumb/");
                    filename = thumb_dir + id;
                    i = new ImageResizer.ImageJob(orig_path, filename, new ImageResizer.Instructions(
                                    "width=75;height=75;format=jpg;mode=max;autorotate=true"));
                    i.CreateParentDirectory = true; //Auto-create the uploads directory.
                    i.Build();

                } else {
                    // doesn't exist.. so... do nothing
                    return new HttpResponseMessage();
                }

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
