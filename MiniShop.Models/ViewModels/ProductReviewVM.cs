using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Models.ViewModels
{
    public class ProductReviewVM
    {
        [ValidateNever]
        public ProductReview ProductReview { get; set; }
        [ValidateNever]
        public List<ProductReview> ProductReviewsList { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }

    }
}
