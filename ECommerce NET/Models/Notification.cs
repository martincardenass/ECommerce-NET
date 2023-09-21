namespace ECommerce_NET.Models
{
    public class Notification
    {
        public int Notification_Id { get; set; }
        public string? Notification_Type { get; set; }
        public string Notification_Value { get; set; }
        public int? Sender_Id { get; set; }
        public int Receiver_Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public User User { get; set; }
    }
}
