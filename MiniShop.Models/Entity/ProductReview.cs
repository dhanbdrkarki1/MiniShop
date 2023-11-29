using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        [DisplayName("Review")]
        [Required]
        public string ReviewText { get; set; }
        [Required]
        public int Rating { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; }

        public DateTime PublishedAt { get; set; }
    }
}
