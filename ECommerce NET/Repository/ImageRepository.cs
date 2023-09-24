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
                var (imageUrl, publicId) = await UploadToCloudinary(image, 0, 0);

                var newImage = new Image
                {
                    Item_Id = itemId,
                    Image_Url = imageUrl,
                    Added = DateTimeOffset.UtcNow,
                    Public_Id = publicId // Cloudinary Id later used for deletion or managing the image
                };
                
                _context.Add(newImage);
                
                _ = await _context.SaveChangesAsync();
                
                imageCollection.Add(newImage);
            }

            return imageCollection;
        }

        public async Task<(List<string> errorMessages, bool result)> DeleteImagesFromCloud(List<Image> images)
        {
            // We assume all images will be deleted
            bool allImagesDeletedSuccessfully = true;

            // List to store publicIds of failed deletions
            List<string> failedDeletions = new();

            foreach (var image in images)
            {
                string publicId = image.Public_Id;

                DeletionParams deletionParams = new(publicId);

                // 3 attempts to delete any of the images
                int maxRetries = 3;

                for(int retryCount = 0; retryCount < maxRetries; retryCount++)
                {
                    try
                    {
                        DeletionResult deletionResult = await _cloudinary.DestroyAsync(deletionParams);

                        if(deletionResult.Result is "ok")
                        {
                            // If it fails, and then it gets deleted in a retry, delete it from de failedDeletions list (havent tested this!)
                            if(failedDeletions.Contains(deletionResult.Result))
                            {
                                failedDeletions.Remove(deletionResult.Result);
                                allImagesDeletedSuccessfully = true;
                            }
                            break; // Deletion success : Exit the retry loop
                        }
                        else
                        {
                            failedDeletions.Add(publicId); // Add the non deleted publicId to the failedDeletions list
                            allImagesDeletedSuccessfully = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        failedDeletions.Add($"Error deleting image with public ID: {publicId}. Error message: {ex.Message}");
                    }
                }
            }

            return (failedDeletions.Distinct().ToList(), allImagesDeletedSuccessfully);
        }

        public async Task<List<Image>> GetImagesByItemId(int itemId) => await _context.Images
            .Where(i => i.Item_Id.Equals(itemId))
            .ToListAsync();

        public async Task<(string imageUrl, string publicId)> UploadToCloudinary(IFormFile file, int width, int height)
        {
            if(file is not null && file.Length > 0)
            {
                await using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    UseFilename = true,
                    UniqueFilename = true,
                    Overwrite = true,
                    Transformation = new Transformation().Quality(50).FetchFormat("webp")
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

                return (uploadResult.SecureUrl.ToString(), uploadResult.PublicId);
            }
            return (null, null);
        }
    }
}
