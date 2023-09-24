namespace ECommerce_NET.Dto
{
    public class DeletionResponseDto
    {
        public bool Deleted { get; set; }
        public string? Message { get; set; }
        public List<string>? FailedPublicIds { get; set; }
    }
}
