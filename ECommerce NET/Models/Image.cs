namespace ECommerce_NET.Models
{
    public class Image
    {
        public int Image_Id { get; set; }
        public int Item_Id { get; set; } // * Items foreign key ref
        public string Image_Url { get; set; }
        public DateTimeOffset Added { get; set; }
        public Item? Item { get; set; }
    }
}
