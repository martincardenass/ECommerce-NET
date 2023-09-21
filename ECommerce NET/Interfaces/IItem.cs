using ECommerce_NET.Dto;
using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface IItem
    {
        Task<ICollection<Item>> GetItems();
        Task<ICollection<Item>> ItemSelectionQuery();
        Task<Item> GetItemById(int id);
        Task<(Item, List<ImageDto>)> NewItem(Item item, List<IFormFile> images);
    }
}
