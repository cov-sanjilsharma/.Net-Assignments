using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Models;

namespace Ecommerce_DBFirst.Services
{
    public interface IProductService
    {
        Task <List<ProductDTO>> GetAllProducts(int? categoryId, string sortBy, string search);
        Task <ProductDTO?> GetProductById(int id);
        Task CreateProduct(ProductDTO productDto);
        Task UpdateProduct(ProductDTO productDto);
        Task DeleteProduct(int id);
        Task<List<Category>> GetAllCategories();   // add this line
    }
}