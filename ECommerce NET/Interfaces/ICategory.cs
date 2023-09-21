using ECommerce_NET.Models;

namespace ECommerce_NET.Interfaces
{
    public interface ICategory
    {
        Task<int> NewCategory(Category category);
        Task<bool> DeleteCategory(Category category);
        Task<Category> GetCategoryById(int id);
    }
}
