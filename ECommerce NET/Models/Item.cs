using System.ComponentModel.DataAnnotations;

namespace ECommerce_NET.Models
{
    public class Item
    {
        public int Item_Id { get; set; }
        [Required(ErrorMessage = "Please provide an item name.")]
        [MaxLength(255, ErrorMessage = "Item name cannot exceed 255 characters.")]
        [MinLength(4, ErrorMessage = "Item name minimum length its 4 characters.")]
        public string Item_Name { get; set; }
        [Required(ErrorMessage = "Please provide a description for this item.")]
        [MaxLength(255, ErrorMessage = "Item description cannot exceed 255 characters.")]
        [MinLength(4, ErrorMessage = "Item description minimum length its 4 characters.")]
        public string Item_Description { get; set; }
        public DateTimeOffset Added { get; set; }
        [Required(ErrorMessage = "Please provide a category for this item.")]
        public int Category_Id { get; set; }
        public Category? Category { get; set; }
        public ICollection<ItemVariant>? Variants { get; set; } // * Variants nav property
        public ICollection<Image>? Images { get; set; } // * Images nav property
        public ICollection<Review>? Reviews { get; set; } // * Reviews navigation property
    }
}
