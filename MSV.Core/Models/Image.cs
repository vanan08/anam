using System;
using System.Collections.Generic;

namespace Accuracy.Core.Models
{
    public partial class Image
    {

        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public string External { get; set; }
        public int Index { get; set; }
    }
}
