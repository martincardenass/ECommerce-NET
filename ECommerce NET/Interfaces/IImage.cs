using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface IImage
    {
        Task<(string imageUrl, string publicId)> UploadToCloudinary(IFormFile file, int width, int height);
        Task<ICollection<Image>> AddImagesToItem(ICollection<IFormFile> images, int imageId);
        Task<List<Image>> GetImagesByItemId(int itemId);
        Task<(List<string> errorMessages, bool result)> DeleteImagesFromCloud(List<Image> images);
    }
}
