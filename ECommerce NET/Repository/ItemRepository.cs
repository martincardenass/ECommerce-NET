
using ECommerce_NET.Data;
using ECommerce_NET.Dto;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Repository
{
    public class ItemRepository : IItem
    {
        private readonly ApplicationDbContext _context;
        private readonly IImage _imageService;
        private readonly IItemVariant _itemVariantService;

        public ItemRepository(ApplicationDbContext context, IImage imageService, IItemVariant itemVariantService)
        {
            _context = context;
            _imageService = imageService;
            _itemVariantService = itemVariantService;
        }

        public async Task<(string deletionResult, List<string> failedPublicIds, bool result)> DeleteItem(Item item)
        {
            var variants = await _itemVariantService.GetItemVariantsByItemId(item.Item_Id);

            if(variants.Count > 0)
            {
                _context.RemoveRange(variants);
            }

            var images = await _imageService.GetImagesByItemId(item.Item_Id);

            string? deletionResult = string.Empty;
            List<string> failedIds = new();

            if(images.Count > 0)
            {
                // Delete images from the cloud too
                var (errorMessages, result) = await _imageService.DeleteImagesFromCloud(images);

                failedIds.AddRange(errorMessages);

                if (result)
                {
                    _context.RemoveRange(images);
                }

                if(errorMessages.Count > 0)
                {
                    deletionResult = "One or more images failed to be deleted from Cloudinary, but they have been deleted from the database.";
                } else if (errorMessages.Count <= 0)
                {
                    deletionResult = "All images have been deleted from Cloudinary and from the database";
                }
            }

            _context.Remove(item);

            return (deletionResult, failedIds, await _context.SaveChangesAsync() > 0);
        }

        public async Task<bool> DoesItemExist(int itemId)
        {
            return await _context.Items
                .AnyAsync(i => i.Item_Id.Equals(itemId));
        }

        public async Task<Item> GetItemById(int id)
        {
            return await _context.Items
                .FirstOrDefaultAsync(i => i.Item_Id.Equals(id));
        }

        public async Task<ICollection<Item>> GetItems()
        {
            var items = await _context.Items
                .OrderByDescending(i => i.Item_Id)
                .Include(c => c.Category)
                .Include(v => v.Variants)
                .ToListAsync();

            var itemDtos = items.Select(item => new Item
            {
                Item_Id = item.Item_Id,
                Item_Name = item.Item_Name,
                Item_Description = item.Item_Description,
                Added = item.Added,
                Category = item.Category,
                Variants = item.Variants.Select(variant => new ItemVariant
                {
                    Variant_Id = variant.Variant_Id,
                    Variant_Name = variant.Variant_Name,
                    Variant_Value = variant.Variant_Value
                }).ToList()
            }).ToList();

            return itemDtos;
        }

        public async Task<ICollection<Item>> ItemSelectionQuery()
        {
            var items = await _context.Items
                .OrderByDescending(i => i.Item_Id)
                .Include(c => c.Category)
                .Include(v => v.Variants)
                .Include(i => i.Images)
                .Include(r => r.Images)
                .ToListAsync();

            return items;
        }

        public async Task<(Item, List<ImageDto>, List<ItemVariantDto>)> NewItem(Item item, List<IFormFile> images, List<ItemVariant> itemVariants)
        {
            var newItem = new Item
            {
                Item_Name = item.Item_Name,
                Item_Description = item.Item_Description,
                Added = DateTime.UtcNow,
                Category_Id = item.Category_Id
            };

            _context.Add(newItem);

            _ = await _context.SaveChangesAsync();

            // Images collection, if images provided.

            List<ImageDto> imageCollection = new();

            if(images is not null && images.Any(i => i.Length > 0))
            {
                var imgs = await _imageService.AddImagesToItem(images, newItem.Item_Id);

                imageCollection.AddRange(imgs.Select(i => new ImageDto
                {
                    Image_Id = i.Image_Id,
                    Image_Url = i.Image_Url,
                    Public_Id = i.Public_Id
                }).ToList());
            }

            // Item variations, if variations provided

            List<ItemVariantDto> variantsCollection = new();
            if(itemVariants is not null && itemVariants.Count > 0)
            {
                var variants = await _itemVariantService.NewItemVariant(newItem.Item_Id, itemVariants);

                variantsCollection.AddRange(variants.Select(v => new ItemVariantDto
                {
                    Variant_Id = v.Variant_Id,
                    Variant_Name = v.Variant_Name,
                    Variant_Value = v.Variant_Value,
                    Item_Id = v.Item_Id
                }));
            }

            return (newItem, imageCollection, variantsCollection);
        }
    }
}
