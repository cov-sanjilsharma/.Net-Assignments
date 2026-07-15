using Ecommerce_DBFirst.Models;

namespace Ecommerce_DBFirst.Services
{
    public interface ICategoryService
    {
        List<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        void CreateCategory(Category category);
        void UpdateCategory(Category category);
    }
}