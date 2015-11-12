using Accuracy.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Accuracy.Web.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public int CategoryID { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public string QuantityPerUnit { get; set; }

        [Required(ErrorMessage = "The field is required")]
        [RegularExpression(@"^[0-9]+(\.[0-9]+)?$", ErrorMessage = "Enter decimal numbers only")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "The field is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter numbers only")]
        public int UnitsInStock { get; set; }

        [Required(ErrorMessage = "The field is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter numbers only")]
        public int UnitsOnOrder { get; set; }

        [Required(ErrorMessage = "The field is required")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Enter numbers only")]
        public int ReorderLevel { get; set; }

        [Required(ErrorMessage = "The field is required")]
        public bool Discontinued { get; set; }

        public Category Category { get; set; }
        public Supplier Supplier { get; set; }
        public IList<Order_Detail> Order_Detailses { get; set; }

        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string Sort { get; set; }
        public bool IsAsc { get; set; }
        public IList<Product> Products { get; set; }

        public ProductModel()
        {
            TotalPages = 0;
            CurrentPage = 0;
            PageSize = 0;
            PageIndex = 0;
            Products = new List<Product>();
        }
    }
}