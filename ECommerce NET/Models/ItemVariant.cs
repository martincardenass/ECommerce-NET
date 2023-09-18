namespace ECommerce_NET.Models
{
    public class ItemVariant
    {
        public int Variant_Id { get; set; }
        public string Variant_Name { get; set; }
        public string Variant_Value { get; set; }
        public int Item_Id { get; set; } // * Foregin key ref items
        public Item? Item { get; set; }
    }
}
