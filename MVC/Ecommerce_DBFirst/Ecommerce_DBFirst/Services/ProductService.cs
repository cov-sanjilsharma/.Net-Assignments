using AutoMapper;
using Ecommerce_DBFirst.Dtos;
using Ecommerce_DBFirst.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Category>> GetAllCategories()
        {
            return await ((IQueryable<Category>)_dbfirstDbContext.Categories).ToListAsync();
        }

        public async Task<List<ProductDTO>> GetAllProducts(int? categoryId, string sortBy, string search)
        {
            _logger.LogInformation("Fetching product list. CategoryId={CategoryId}, SortBy={SortBy}, Search={Search}", categoryId, sortBy, search);

            var categories = await ((IQueryable<Category>)_dbfirstDbContext.Categories).ToListAsync();
            var products = _dbfirstDbContext.Products.AsQueryable();

            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryId == categoryId);

            if (sortBy == "price")
                products = products.OrderBy(p => p.Price);

            if (!string.IsNullOrEmpty(search))
                products = products.Where(p => p.ProductName.Contains(search));

            var productDtos = _mapper.Map<List<ProductDTO>>(await products.ToListAsync());

            foreach (var dto in productDtos)
                dto.CategoryName = categories.FirstOrDefault(c => c.CategoryId == dto.CategoryId)?.CategoryName;

            return productDtos;
        }

        public async Task<ProductDTO?> GetProductById(int id)
        {
            var product = await _dbfirstDbContext.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found. ProductId={ProductId}", id);
                return null;
            }

            var productDto = _mapper.Map<ProductDTO>(product);
            var category = await _dbfirstDbContext.Categories.FindAsync(productDto.CategoryId);
            productDto.CategoryName = category?.CategoryName;
            return productDto;
        }

        public async Task CreateProduct(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Products>(productDto);
                await _dbfirstDbContext.Products.AddAsync(product);
                await _dbfirstDbContext.SaveChangesAsync();
                _logger.LogInformation("Product created successfully. ProductId={ProductId}, ProductName={ProductName}", product.ProductId, product.ProductName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating product. ProductName={ProductName}", productDto.ProductName);
                throw;
            }
        }

        public async Task UpdateProduct(ProductDTO productDto)
        {
            try
            {
                var product = _mapper.Map<Products>(productDto);
                _dbfirstDbContext.Products.Update(product);
                await _dbfirstDbContext.SaveChangesAsync();
                _logger.LogInformation("Product updated successfully. ProductId={ProductId}", product.ProductId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product. ProductId={ProductId}", productDto.ProductId);
                throw;
            }
        }

        public async Task DeleteProduct(int id)
        {
            var product = await _dbfirstDbContext.Products.FindAsync(id);
            if (product == null)
            {
                _logger.LogWarning("Product not found for delete. ProductId={ProductId}", id);
                throw new KeyNotFoundException($"Product {id} not found.");
            }

            try
            {
                _dbfirstDbContext.Products.Remove(product);
                await _dbfirstDbContext.SaveChangesAsync();
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