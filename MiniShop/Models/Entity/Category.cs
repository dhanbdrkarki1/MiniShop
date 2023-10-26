using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MiniShop.Models.Entity
{
    public class Category
    {
        [Key]

        public int CategoryId { get; set; }

        [DisplayName("Category Name")]
        [Required(ErrorMessage = "Required")]
        [MaxLength(50)]
        public string Name { get; set; }

        [DisplayName("Category Description")]
        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
