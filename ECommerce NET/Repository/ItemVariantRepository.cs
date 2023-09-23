using ECommerce_NET.Data;
using ECommerce_NET.Dto;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce_NET.Repository
{
    public class ItemVariantRepository : IItemVariant
    {
        private readonly ApplicationDbContext _context;

        public ItemVariantRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ItemVariant>> GetItemVariantsByItemId(int itemId) => await _context.ItemVariants
                .Where(i => i.Item_Id.Equals(itemId))
                .ToListAsync();

        public async Task<List<ItemVariantDto>> NewItemVariant(int itemId, List<ItemVariant> variants)
        {
            foreach(var variant in variants)
            {
                variant.Item_Id = itemId;
                _context.Add(variant);
            }

            _ = await _context.SaveChangesAsync();

            List<ItemVariantDto> itemVariantCollection = new();

            foreach (var variant in variants)
            {
                var newVariantDto = new ItemVariantDto
                {
                    Variant_Id = variant.Variant_Id,
                    Variant_Name = variant.Variant_Name,
                    Variant_Value = variant.Variant_Value,
                    Item_Id = variant.Item_Id,
                };

                itemVariantCollection.Add(newVariantDto);
            }

            return itemVariantCollection;
        }
    }
}
