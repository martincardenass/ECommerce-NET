namespace ECommerce_NET.Interfaces
{
    public interface IImage
    {
        Task<string> UploadToCloudinary(IFormFile file, int width, int height);
    }
}
