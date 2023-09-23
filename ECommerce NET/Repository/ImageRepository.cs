using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using ECommerce_NET.Data;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<ICollection<Image>> AddImagesToItem(ICollection<IFormFile> images, int itemId)
        {
            List<Image> imageCollection = new();
            foreach (var image in images)
            {
                // 0s for width and height for no crop
                string? imageUrl = await UploadToCloudinary(image, 0, 0);
                var newImage = new Image
                {
                    Item_Id = itemId,
                    Image_Url = imageUrl,
                    Added = DateTimeOffset.UtcNow
                };
                
                _context.Add(newImage);
                
                _ = await _context.SaveChangesAsync();
                
                imageCollection.Add(newImage);
            }

            return imageCollection;
        }

        public Task<bool> DeleteImagesFromCloud(List<Image> images)
        {
            //foreach (var image in images)
            //{
            //    string publicId = image.;
            //    var deletionResult = await _cloudinary.DestroyAsync(image);
            //}
            throw new NotImplementedException();
        }

        public async Task<List<Image>> GetImagesByItemId(int itemId) => await _context.Images
            .Where(i => i.Item_Id.Equals(itemId))
            .ToListAsync();

        public async Task<string> UploadToCloudinary(IFormFile file, int width, int height)
        {
            if(file is not null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true
                };

                // If width and height arguements are given crop the image accordingly
                if(width > 0 && height > 0)
                {
                    uploadParams.Transformation = new Transformation().Width(width).Height(height).Crop("fill");
                }

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if(uploadResult.Error != null)
                {
                    throw new Exception(uploadResult.Error.Message);
                }

                return uploadResult.SecureUrl.ToString();
            }
            return null;
        }
    }
}
