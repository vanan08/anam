using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

#pragma warning disable 1591
namespace Accuracy.Web.Common
{
    public class FileDesc
    {
        /// <summary>
        /// Tên file
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Đường dẫn đến file
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Kích thước file
        /// </summary>
        public long Size { get; set; }

        public FileDesc(string n, string p, long s)
        {
            Name = n;
            Path = p;
            Size = s;
        }
    }
}