using Ecommerce_DBFirst.Models;

namespace Ecommerce_DBFirst.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllCategories();
        Task<Category?> GetCategoryById(int id);
        Task CreateCategory(Category category);
        Task UpdateCategory(Category category);
    }
}