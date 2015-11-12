using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Accuracy.Web.Models
{
    public class ImageModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string External { get; set; }
        public int Index { get; set; }
    }
}