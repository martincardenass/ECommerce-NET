using System.ComponentModel.DataAnnotations;

namespace ECommerce_NET.Models
{
    public class Category
    {
        public int Category_Id { get; set; }
        [Required(ErrorMessage = "Please specify a name.")]
        [MaxLength(255, ErrorMessage = "Category name cannot exceed 255 characters.")]
        [MinLength(4, ErrorMessage = "Category name minimum length its 4 characters.")]
        public string Category_Name { get; set; }
        public DateTimeOffset Added { get; set; }
    }
}
