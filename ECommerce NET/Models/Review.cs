namespace ECommerce_NET.Models
{
    public class Review
    {
        public int Review_Id { get; set; }
        public string Review_Title { get; set; }
        public string Review_Content { get; set; }
        public DateTimeOffset Added { get; set; }
        public int User_Id { get; set; } // * Ref to users table
        public int Item_Id { get; set; } // * Ref to items table
        public User? User { get; set; }
        public Item? Item { get; set; }
    }
}
