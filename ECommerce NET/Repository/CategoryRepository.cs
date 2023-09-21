using ECommerce_NET.Data;
using ECommerce_NET.Interfaces;
using ECommerce_NET.Models;

namespace ECommerce_NET.Repository
{
    public class CategoryRepository : ICategory
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteCategory(Category category)
        {
            _context.Remove(category);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Category> GetCategoryById(int id)
        {
            return await _context.Categories
                .FindAsync(id);
        }

        public async Task<int> NewCategory(Category category)
        {
            var newCategory = new Category
            {
                Category_Name = category.Category_Name,
                Added = DateTimeOffset.UtcNow
            };

            _context.Add(newCategory);

            _ = await _context.SaveChangesAsync();

            return newCategory.Category_Id;
        }
    }
}
