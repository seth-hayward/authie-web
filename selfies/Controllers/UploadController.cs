using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ImageResizer;
using selfies.Models;

namespace selfies.Controllers
{
    public class UploadController : ApiController
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

        
        public async Task<HttpResponseMessage> PostFile(string key)
        {
            string user_id = User.Identity.Name;
            handle logged_in = (from handle r in db.handles where r.userGuid.Equals(User.Identity.Name) select r).FirstOrDefault();
            thread referring_thread = (from thread r in db.threads where r.groupKey == key select r).FirstOrDefault();

            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string orig_dir = HttpContext.Current.Server.MapPath("~/snaps/orig/");
            string sz500_dir = HttpContext.Current.Server.MapPath("~/snaps/500/");
            string sz640_dir = HttpContext.Current.Server.MapPath("~/snaps/640/");
            string thumb_dir = HttpContext.Current.Server.MapPath("~/snaps/thumb/");

            var provider = new MultipartFormDataStreamProvider(orig_dir);

            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body

                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);
                
                // This illustrates how to get the form data.
                foreach (var item_key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(item_key))
                    {
                        sb.Append(string.Format("{0}: {1}\n", item_key, val));
                    }
                }

                selfy self = new selfy();

                // This illustrates how to get the file names for uploaded files.
                foreach (var file in provider.FileData)
                {
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);
                    sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));

                    thread group_key = (from m in db.threads where m.groupKey.StartsWith(key) select m).FirstOrDefault();

                    self.dateCreated = DateTime.UtcNow;
                    self.selfieGuid = group_key.groupKey;
                    self.userGuid = "1";
                    db.selfies.Add(self);

                    db.SaveChanges();

                    referring_thread.uploadSuccess = 1;

                    // update the thread so it knows that
                    // an image has been successfully uploaded to it
                    db.threads.Attach(referring_thread);
                    var updated_thread = db.Entry(referring_thread);
                    updated_thread.Property(e => e.uploadSuccess).IsModified = true;
                    db.SaveChanges();

                    string copied_orig_to_path = orig_dir + self.selfieGuid;

                    if (Directory.Exists(orig_dir) == false)
                    {
                        Directory.CreateDirectory(orig_dir);
                    }
                    fileInfo.CopyTo(copied_orig_to_path);

                    // generate 500px version
                    string fileName = sz500_dir + self.selfieGuid;

                    ImageResizer.ImageJob i = new ImageResizer.ImageJob(copied_orig_to_path, fileName, new ImageResizer.Instructions(
                                    "width=500;height=900;format=jpg;mode=max;autorotate=true"));
                    i.CreateParentDirectory = true; //Auto-create the uploads directory.
                    i.Build();

                    // generate 640px version (retina display)
                    fileName = sz640_dir + self.selfieGuid;

                    i = new ImageResizer.ImageJob(copied_orig_to_path, fileName, new ImageResizer.Instructions(
                                    "width=640;height=900;format=jpg;mode=max;autorotate=true"));
                    i.CreateParentDirectory = true; //Auto-create the uploads directory.
                    i.Build();

                    // generate thumb version
                    fileName = thumb_dir + self.selfieGuid;

                    i = new ImageResizer.ImageJob(copied_orig_to_path, fileName, new ImageResizer.Instructions(
                                    "width=75;height=75;format=jpg;mode=max;autorotate=true"));
                    i.CreateParentDirectory = true; //Auto-create the uploads directory.
                    i.Build();

                    fileInfo.Delete();
                    
                }

                var response = new HttpResponseMessage()
                {
                    Content = new StringContent(self.selfieGuid.ToString())
                }; 
                return response;

            }
            catch (System.Exception e)
            {
                string result = e.Message.ToString();
                if (e.InnerException != null)
                {
                    result = result + " - " + e.InnerException.Message.ToString();
                }

                var response = new HttpResponseMessage()
                {
                    Content = new StringContent(e.Message.ToString())
                };
                return response;
            }
        }

    }
}
