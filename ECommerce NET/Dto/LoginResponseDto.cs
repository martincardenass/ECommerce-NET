namespace ECommerce_NET.Dto
{
    public class LoginResponseDto
    {
        public bool IsAuthenticated { get; set; }
        public string Message { get; set; }
        public UserLimitedDto User { get; set; }
    }
}
