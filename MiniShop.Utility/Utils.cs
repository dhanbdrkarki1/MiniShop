using MiniShop.Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Utility
{
    public class Utils
    {
        // compute the average rating

        public decimal CalculateOverallRating(List<ProductReview> reviews)
        {
            if (reviews == null || reviews.Count == 0)
            {
                return 0; 
            }

            decimal totalRating = 0;

            foreach (var review in reviews)
            {
                totalRating += review.Rating;
            }

            decimal averageRating = totalRating / reviews.Count;

            return Math.Round(averageRating, 1);
        }




    }
}
