using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface IItem
    {
        Task<ICollection<Item>> GetItems();
        Task<int> NewItem(Item item);
    }
}
