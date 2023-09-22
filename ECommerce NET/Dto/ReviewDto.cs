namespace ECommerce_NET.Dto
{
    public class ReviewDto
    {
        public int Review_Id { get; set; }
        public string? Review_Title { get; set; }
        public string? Review_Content { get; set; }
        public DateTimeOffset? Modified { get; set; }
    }
}
