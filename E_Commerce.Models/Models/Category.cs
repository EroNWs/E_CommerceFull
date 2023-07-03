using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30, ErrorMessage = "Category Name length must be less than 30")]
        public string Name { get; set; }
        [DisplayName("Display Order")]
        [Range(1, 100, ErrorMessage = "Display Order must be between 1-100")]
        public int DisplayOrder { get; set; }
    }
}
