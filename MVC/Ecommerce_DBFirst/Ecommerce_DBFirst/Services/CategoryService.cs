using AutoMapper;
using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Models;

namespace Ecommerce_DBFirst.Services
{
    public class ProductService : IProductService
    {
        private readonly EcommerceDbfirstDbContext _dbfirstDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductService> _logger;

        public ProductService(EcommerceDbfirstDbContext dbfirstDbContext, IMapper mapper, ILogger<ProductService> logger)
        {
            _dbfirstDbContext = dbfirstDbContext;
            _mapper = mapper;
            _logger = logger;
        }

        public List<Category> GetAllCategories()
        {
            return _dbfirstDbContext.Categories.ToList();
        }
        public List<ProductDTO> GetAllProducts(int? categoryId, string sortBy, string search)
        {
            _logger.LogInformation("Fetching product list. CategoryId={CategoryId}, SortBy={SortBy}, Search={Search}", categoryId, sortBy, search);

            var categories = _dbfirstDbContext.Categories.ToList();
            var products = _dbfirstDbContext.Products.AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId);

            if (sortBy == "price")
                products = products.OrderBy(p => p.Price);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.ProductName.Contains(search));

            var productDtos = _mapper.Map<List<ProductDTO>>(products.ToList());

            foreach (var dto in productDtos)
                dto.CategoryName = categories.FirstOrDefault(c => c.CategoryId == dto.CategoryId)?.CategoryName;

            return productDtos;
        }

        public ProductDTO? GetProductById(int id)
        {
            var product = _dbfirstDbContext.Products.Find(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found. ProductId={ProductId}", id);
                return null;
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            var category = _dbfirstDbContext.Categories.Find(productDto.CategoryId);
            productDto.CategoryName = category?.CategoryName;
            return productDto;
        }

        public void CreateProduct(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Products>(productDto);
                _dbfirstDbContext.Products.Add(product);
                _dbfirstDbContext.SaveChanges();
                _logger.LogInformation("Product created successfully. ProductId={ProductId}, ProductName={ProductName}", product.ProductId, product.ProductName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product. ProductName={ProductName}", productDto.ProductName);
                throw; // let the controller decide how to respond to the user
            }
        }

        public void UpdateProduct(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Products>(productDto);
                _dbfirstDbContext.Products.Update(product);
                _dbfirstDbContext.SaveChanges();
                _logger.LogInformation("Product updated successfully. ProductId={ProductId}", product.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product. ProductId={ProductId}", productDto.ProductId);
                throw;
            }
        }

        public void DeleteProduct(int id)
        {
            var product = _dbfirstDbContext.Products.Find(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for delete. ProductId={ProductId}", id);
                throw new KeyNotFoundException($"Product {id} not found.");
            }

            try
            {
                _dbfirstDbContext.Products.Remove(product);
                _dbfirstDbContext.SaveChanges();
                _logger.LogInformation("Product deleted successfully. ProductId={ProductId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product. ProductId={ProductId}", id);
                throw;
            }
        }
    }
}