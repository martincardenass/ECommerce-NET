using ECommerce_NET.Data;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Repository
{
    public class ItemRepository : IItem
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<int> NewItem(Item item)
        {
            var newItem = new Item()
            {
                Item_Name = item.Item_Name,
                Item_Description = item.Item_Description,
                Added = DateTime.UtcNow,
                Category_Id = item.Category_Id
            };

            _context.Add(newItem);

            _ = await _context.SaveChangesAsync();

            return newItem.Item_Id;
        }
    }
}
