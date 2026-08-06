using Ecommerce.Api.Controllers;
using Ecommerce.Api.Data;
using Ecommerce.Api.Dtos;
using Ecommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Ecommerce_DBFirst.NUnitTests
{
    [TestFixture]
    public class InventoryControllerTests
    {
        private static InventoryDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new InventoryDbContext(options);
        }

        [Test]
        public async Task GetAll_ReturnsAllInventoryRecords()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var product = new Product { ProductName = "Test Chair", Price = 2500m, StockQuantit = 10, CategoryId = 1 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            context.Inventories.Add(new Inventory { ProductId = product.ProductId, Quantity = 15, LastUpdated = DateTime.Now });
            await context.SaveChangesAsync();

            var controller = new InventoryController(context);

            // Act
            var actionResult = await controller.GetAll();

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            var items = okResult!.Value as IEnumerable<InventoryDto>;
            Assert.That(items, Is.Not.Null);
            Assert.That(items!.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetByProductId_ProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var controller = new InventoryController(context);

            // Act
            var actionResult = await controller.GetByProductId(999);

            // Assert
            Assert.That(actionResult.Result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Create_ValidInventory_AddsRecordAndReturnsCreatedAt()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var product = new Product { ProductName = "Test Lamp", Price = 899m, StockQuantit = 30, CategoryId = 2 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var controller = new InventoryController(context);
            var dto = new InventoryCreateDto { ProductId = product.ProductId, Quantity = 20 };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            var createdResult = actionResult.Result as CreatedAtActionResult;
            Assert.That(createdResult, Is.Not.Null);
            var returnedDto = createdResult!.Value as InventoryDto;
            Assert.That(returnedDto, Is.Not.Null);
            Assert.That(returnedDto!.Quantity, Is.EqualTo(20));

            // double-check it's actually saved in the DB, not just returned
            Assert.That(await context.Inventories.CountAsync(), Is.EqualTo(1));
        }

        [Test]
        public async Task Create_NegativeQuantity_ReturnsBadRequest()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var controller = new InventoryController(context);
            var dto = new InventoryCreateDto { ProductId = 1, Quantity = -5 };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            Assert.That(actionResult.Result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task UpdateStock_ExistingRecord_UpdatesQuantity()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var product = new Product { ProductName = "Test Desk", Price = 4999m, StockQuantit = 5, CategoryId = 3 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var inventory = new Inventory { ProductId = product.ProductId, Quantity = 5, LastUpdated = DateTime.Now };
            context.Inventories.Add(inventory);
            await context.SaveChangesAsync();

            var controller = new InventoryController(context);
            var dto = new InventoryUpdateDto { Quantity = 50 };

            // Act
            var result = await controller.UpdateStock(inventory.InventoryId, dto);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            var updated = await context.Inventories.FindAsync(inventory.InventoryId);
            Assert.That(updated!.Quantity, Is.EqualTo(50));
        }

        [Test]
        public async Task Delete_ExistingRecord_RemovesIt()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var product = new Product { ProductName = "Test Shelf", Price = 1200m, StockQuantit = 8, CategoryId = 4 };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var inventory = new Inventory { ProductId = product.ProductId, Quantity = 8, LastUpdated = DateTime.Now };
            context.Inventories.Add(inventory);
            await context.SaveChangesAsync();

            var controller = new InventoryController(context);

            // Act
            var result = await controller.Delete(inventory.InventoryId);

            // Assert
            Assert.That(result, Is.TypeOf<NoContentResult>());
            Assert.That(await context.Inventories.CountAsync(), Is.EqualTo(0));
        }

        [Test]
        public async Task Delete_NonExistentRecord_ReturnsNotFound()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var controller = new InventoryController(context);

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }
    }
}
