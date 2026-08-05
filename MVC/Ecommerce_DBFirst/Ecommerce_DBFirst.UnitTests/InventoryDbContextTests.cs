using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ecommerce_DBFirst.UnitTests
{
    public class InventoryDbContextTests
    {
        private static InventoryDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new InventoryDbContext(options);
        }

        [Fact]
        public async Task AddingProduct_PersistsAndCanBeRetrieved()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var product = new Product
            {
                ProductName = "Test Keyboard",
                Price = 1999.00m,
                StockQuantit = 50,
                CategoryId = 1
            };

            // Act
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Assert
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductName == "Test Keyboard");
            Assert.NotNull(savedProduct);
            Assert.Equal(1999.00m, savedProduct!.Price);
        }

        [Fact]
        public async Task AddingInventory_LinksCorrectlyToItsProduct()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var product = new Product
            {
                ProductName = "Test Mouse",
                Price = 599.00m,
                StockQuantit = 100,
                CategoryId = 2
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var inventory = new Inventory
            {
                ProductId = product.ProductId,
                Quantity = 25,
                LastUpdated = DateTime.Now
            };

            // Act
            context.Inventories.Add(inventory);
            await context.SaveChangesAsync();

            // Assert
            var savedInventory = await context.Inventories
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.ProductId == product.ProductId);

            Assert.NotNull(savedInventory);
            Assert.Equal(25, savedInventory!.Quantity);
            Assert.Equal("Test Mouse", savedInventory.Product.ProductName);
        }

        [Fact]
        public async Task NoProductsExist_ProductsListIsEmpty()
        {
            // Arrange
            await using var context = CreateInMemoryContext();

            // Act
            var products = await context.Products.ToListAsync();

            // Assert
            Assert.Empty(products);
        }
    }
}