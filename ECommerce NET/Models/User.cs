namespace ECommerce_NET.Models
{
    public class User
    {
        public int User_Id { get; set; }
        public string Username { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string Password { get; set; }
        public string? Gender { get; set; }
        public string? Profile_Picture { get; set; }
        public DateTime Last_Login { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public ICollection<Review>? Reviews { get; set; } // * Reviews navigation property
    }
}
