using Accuracy.Core.Models;
using Accuracy.Web.Common;
using Accuracy.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Accuracy.Web.Controllers
{
    /// <summary>
    /// API upload file lên hệ thống
    /// </summary>
    public class UploadingController : ApiController
    {

        //  POST api/Uploading/Post
        
        [ResponseType(typeof(FileDesc))]
        [Authorize]
        [System.Web.Http.HttpPost]
        public Task<IEnumerable<FileDesc>> Post()
        {
            var folderName = "uploads";
            string fileName = "";
            var PATH = HttpContext.Current.Server.MapPath("~/" + folderName);

            if (!Directory.Exists(PATH))
                Directory.CreateDirectory(PATH);

            var rootUrl = Request.RequestUri.AbsoluteUri.Replace(Request.RequestUri.AbsolutePath, String.Empty);
          
          
            if (Request.Content.IsMimeMultipartContent())
            {
                var streamProvider = new CustomMultipartFormDataStreamProvider(PATH);
                var task = Request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith<IEnumerable<FileDesc>>(t =>
                {

                    if (t.IsFaulted || t.IsCanceled)
                    {
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal server error");
                        response.Headers.Add("Error-Code", "EUFILE");
                        throw new HttpResponseException(response);
                    }

                    var fileInfo = streamProvider.FileData.Select(i =>
                    {
                        var info = new FileInfo(i.LocalFileName);
                        string extention = Path.GetExtension(info.Name);
                        string newFileName = info.Name + "-" + fileName + extention;
                        if (info.Exists)
                        {
                            if (File.Exists(PATH + "/" + newFileName))
                            {
                                File.Delete(PATH + "/" + newFileName);
                            }

                            info.MoveTo(PATH + "/" + newFileName);
                        }

                        if (fileName.Equals("")) fileName = info.Name;

                        string fileUrl = rootUrl + "/" + folderName + "/" + newFileName;
                        
                        //Update Avatar
                        using (NORTHWNDContext context = new NORTHWNDContext())
                        {
                            //TODO: insert to DB
                        }

                        return new FileDesc(fileName, fileUrl, info.Length / 1024);
                    });

                    return fileInfo;
                });

                return task;
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted");
                response.Headers.Add("Error-Code", "EUFILE");
                throw new HttpResponseException(response);
            }
        }

        private string ToString(string arg)
        {
            return arg.ToString();
        }


    }
}