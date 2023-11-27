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
        public string Title { get; set; }   
        public List<ProductReview> ProductReviewsList { get; set; }

        public string Review { get; set; }

        public int ProductId { get; set; }
        public int Rating { get; set; }

    }
}
