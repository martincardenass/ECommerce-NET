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

        public ItemRepository(ApplicationDbContext context, IImage imageService)
        {
            _context = context;
            _imageService = imageService;
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

        public async Task<(Item, List<ImageDto>)> NewItem(Item item, List<IFormFile> images)
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

            List<ImageDto> imageCollection = new();

            if(images is not null && images.Any(i => i.Length > 0))
            {
                var imgs = await _imageService.AddImagesToItem(images, newItem.Item_Id);

                var imagesToDto = imgs.Select(i => new ImageDto
                {
                    Image_Id = i.Item_Id,
                    Image_Url = i.Image_Url
                }).ToList();

                imageCollection.AddRange(imagesToDto);
            }

            return (newItem, imageCollection);
        }
    }
}
