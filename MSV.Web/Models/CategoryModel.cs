using Accuracy.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace CMC.Northwind.Models
{
    public class CategoryModel
    {
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public string CategoryName { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public byte[] Picture { get; set; }

        public IList<Product> Products { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Sort { get; set; }
        public bool IsAsc { get; set; }
        public IList<Category> Categories { get; set; }

        public CategoryModel()
        {
            TotalPages = 0;
            CurrentPage = 0;
            PageSize = 0;
            PageIndex = 0;
            Categories = new List<Category>();
        }
    }
}
