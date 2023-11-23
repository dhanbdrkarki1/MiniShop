using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniShop.Models.Entity
{
    public class SubCategory
    {
        [Key]

        public int SubCategoryId { get; set; }

        [DisplayName("Sub Category Name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(80)]
        public string Name { get; set; }

        [DisplayName("Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        [ValidateNever]
        public Category Category { get; set; }

        [DisplayName("Sub Category Description")]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
