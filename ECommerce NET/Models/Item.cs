namespace ECommerce_NET.Models
{
    public class Item
    {
        public int Item_Id { get; set; }
        public string Item_Name { get; set; }
        public string Item_Description { get; set; }
        public DateTime Added { get; set; }
        public int Category_Id { get; set; }
        public Category? Category { get; set; }
        public ICollection<ItemVariant> Variants { get; set; } // * Variants nav property
        public ICollection<Image> Images { get; set; } // * Images nav property
        public ICollection<Review>? Reviews { get; set; } // * Reviews navigation property
    }
}
