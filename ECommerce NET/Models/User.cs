namespace ECommerce_NET.Models
{
    public class User
    {
        public int User_Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string? First_Name { get; set; }
        public string? Last_Name { get; set; }
        public string Password { get; set; }
        public string? Gender { get; set; }
        public string? Profile_Picture { get; set; }
        public DateTimeOffset Last_Login { get; set; }
        public string Email { get; set; }
        public DateTimeOffset Created { get; set; }
        public ICollection<Review>? Reviews { get; set; } // Reviews navigation property
        public ICollection<Notification>? Notifications { get; set; } // Notifications navigation property
        public bool? Lockout_Enabled { get; set; }
        public int? Login_Attempts { get; set; }
        public DateTimeOffset? Locked_Until { get; set; }
    }
}
