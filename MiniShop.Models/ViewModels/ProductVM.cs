using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Models.ViewModels
{
    public class ProductVM
    {
        [ValidateNever]
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> SubCategoryList { get; set; }

        public ProductReviewVM ProductReviewVM { get; set; }
        [ValidateNever]
        public ShoppingCart ShoppingCart { get; set; }
        [ValidateNever]
        public bool HasPurchasedProduct { get; set; }

        [ValidateNever]
        public decimal OverallRating { get; set; }

        [ValidateNever]
        public int UnitSold { get; set; } 

    }
}
