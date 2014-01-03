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


        public async Task<HttpResponseMessage> PostFile()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            string root = HttpContext.Current.Server.MapPath("~/App_Data/");
            var provider = new MultipartFormDataStreamProvider(root);

            try
            {
                StringBuilder sb = new StringBuilder(); // Holds the response body

                // Read the form data and return an async task.
                await Request.Content.ReadAsMultipartAsync(provider);

                // This illustrates how to get the form data.
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        sb.Append(string.Format("{0}: {1}\n", key, val));
                    }
                }

                selfy self = new selfy();

                // This illustrates how to get the file names for uploaded files.
                foreach (var file in provider.FileData)
                {
                    FileInfo fileInfo = new FileInfo(file.LocalFileName);
                    sb.Append(string.Format("Uploaded file: {0} ({1} bytes)\n", fileInfo.Name, fileInfo.Length));

                    self.dateCreated = DateTime.UtcNow;
                    self.selfieGuid = System.Guid.NewGuid().ToString();
                    self.userGuid = "1";
                    db.selfies.Add(self);
                    db.SaveChanges();

                    string dir_path = root + "snaps/";

                    if (Directory.Exists(dir_path) == false)
                    {
                        Directory.CreateDirectory(dir_path);
                    }
                    fileInfo.CopyTo(dir_path + self.selfieGuid);
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
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }


    }
}
