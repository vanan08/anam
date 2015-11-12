using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

#pragma warning disable 1591
namespace Accuracy.Web.Common
{
    public class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
    {
        string fileName;
        public CustomMultipartFormDataStreamProvider(string path)
            : base(path)
        {
        }

        public CustomMultipartFormDataStreamProvider(string path, string fileName)
            : base(path)
        {
            this.fileName = fileName;
        }

        public override string GetLocalFileName(System.Net.Http.Headers.HttpContentHeaders headers)
        {
            //var name = !string.IsNullOrWhiteSpace(headers.ContentDisposition.FileName) ? headers.ContentDisposition.FileName : "NoName";
            var name = !string.IsNullOrWhiteSpace(this.fileName) ? this.fileName : headers.ContentDisposition.FileName;
            return name.Replace("\"", string.Empty); //this is here because Chrome submits files in quotation marks which get treated as part of the filename and get escaped
        }
    }
}