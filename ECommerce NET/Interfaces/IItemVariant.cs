using ECommerce_NET.Dto;
using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface IItemVariant
    {
        Task<List<ItemVariantDto>> NewItemVariant(int itemId, List<ItemVariant> variants);
        Task<List<ItemVariant>> GetItemVariantsByItemId(int itemId);
    }
}
