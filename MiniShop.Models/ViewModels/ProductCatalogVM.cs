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
    public class ProductCatalogVM
    {
        public List<Product> Products { get; set; } 
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SubCategory> SubCategoryList { get; set; }
        public Dictionary<int, decimal> OverallRatings { get; set; }

        public PaginationInfo PaginationInfo { get; set; }

    }
}
