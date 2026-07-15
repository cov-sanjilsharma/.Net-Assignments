using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Models;

namespace Ecommerce_DBFirst.Services
{
    public interface IProductService
    {
        List<ProductDTO> GetAllProducts(int? categoryId, string sortBy, string search);
        ProductDTO? GetProductById(int id);
        void CreateProduct(ProductDTO productDto);
        void UpdateProduct(ProductDTO productDto);
        void DeleteProduct(int id);
        List<Category> GetAllCategories();   // add this line
    }
}