using Ecommerce.Api.Data;
using Ecommerce.Api.Models;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Ecommerce_DBFirst.NUnitTests
{
    [TestFixture]
    public class InventoryDbContextTests
    {
        private static InventoryDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new InventoryDbContext(options);
        }
        [Test]
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
            Assert.That(savedProduct, Is.Not.Null);
            Assert.That(savedProduct!.Price, Is.EqualTo(1999.00m));
        }
        [Test]
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

            Assert.That(savedInventory, Is.Not.Null);
            Assert.That(savedInventory!.Quantity, Is.EqualTo(25));
            Assert.That(savedInventory.Product.ProductName, Is.EqualTo("Test Mouse"));
        }
        [Test]
        public async Task NoProductsExist_ProductsListIsEmpty()
        {
            // Arrange
            await using var context = CreateInMemoryContext();

            // Act
            var products = await context.Products.ToListAsync();

            // Assert
            Assert.That(products, Is.Empty);
        }
    }
}
