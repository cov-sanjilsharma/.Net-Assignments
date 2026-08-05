using Ecommerce_DBFirst.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_DBFirst.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly EcommerceDbfirstDbContext _dbfirstDbContext;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(EcommerceDbfirstDbContext dbfirstDbContext, ILogger<CategoryService> logger)
        {
            _dbfirstDbContext = dbfirstDbContext;
            _logger = logger;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            _logger.LogInformation("Fetching category list.");
            return await _dbfirstDbContext.Categories.ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            var category = await _dbfirstDbContext.Categories.FindAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category not found. CategoryId={CategoryId}", id);
            }
            return category;
        }

        public async Task CreateCategory(Category category)
        {
            try
            {
                await _dbfirstDbContext.Categories.AddAsync(category);
                await _dbfirstDbContext.SaveChangesAsync();
                _logger.LogInformation("Category created successfully. CategoryId={CategoryId}, CategoryName={CategoryName}", category.CategoryId, category.CategoryName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category. CategoryName={CategoryName}", category.CategoryName);
                throw;
            }
        }

        public async Task UpdateCategory(Category category)
        {
            try
            {
                _dbfirstDbContext.Categories.Update(category);
                await _dbfirstDbContext.SaveChangesAsync();
                _logger.LogInformation("Category updated successfully. CategoryId={CategoryId}", category.CategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category. CategoryId={CategoryId}", category.CategoryId);
                throw;
            }
        }
    }
}