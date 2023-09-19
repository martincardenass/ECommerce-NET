using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce_NET.Data;
using ECommerce_NET.Interfaces;

namespace ECommerce_NET.Repository
{
    public class ImageRepository : IImage
    {
        private readonly ApplicationDbContext _context;
        private readonly Cloudinary _cloudinary;

        public ImageRepository(ApplicationDbContext context, Cloudinary cloudinary)
        {
            _context = context;
            _cloudinary = cloudinary;
        }
        public async Task<string> UploadToCloudinary(IFormFile file, int width, int height)
        {
            if(file != null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                };

                if(width > 0 && height > 0)
                {
                    uploadParams.Transformation = new Transformation().Width(width).Height(height).Crop("fill");
                }

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if(uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult.SecureUri.ToString();
            }
            return null;
        }
    }
}
