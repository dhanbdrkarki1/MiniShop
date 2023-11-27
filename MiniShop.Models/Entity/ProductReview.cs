using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniShop.Models.Entity
{
    public class ProductReview
    {
        [Key]
        public int ProductReviewId { get; set; }
        public string ReviewText { get; set; }

        public int Rating { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public DateTime PublishedAt { get; set; }
    }
}
