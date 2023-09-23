using ECommerce_NET.Models;

namespace ECommerce_NET.Dto
{
    public class NewItemDto
    {
        public int Item_Id { get; set; }
        public string Item_Name { get; set; }
        public string Item_Description { get; set; }
        public DateTimeOffset Added { get; set; }
        public int Category_Id { get; set; }
        public ICollection<ImageDto>? Images { get; set; }
        public ICollection<ItemVariantDto>? Variants { get; set; }
    }
}
