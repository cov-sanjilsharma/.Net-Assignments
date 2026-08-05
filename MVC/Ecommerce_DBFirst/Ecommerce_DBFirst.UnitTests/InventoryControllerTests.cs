using Ecommerce.Api.Controllers;
using Ecommerce.Api.Data;
using Ecommerce.Api.Dtos;
using Ecommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ecommerce_DBFirst.UnitTests
{
    public class InventoryControllerTests
    {
        // Same helper pattern as InventoryDbContextTests:
        // fresh, isolated fake database per test.
        private static InventoryDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new InventoryDbContext(options);
        }

        [Fact]
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
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var items = Assert.IsAssignableFrom<IEnumerable<InventoryDto>>(okResult.Value);
            Assert.Single(items);
        }

        [Fact]
        public async Task GetByProductId_ProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var controller = new InventoryController(context);

            // Act
            var actionResult = await controller.GetByProductId(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(actionResult.Result);
        }

        [Fact]
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
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var returnedDto = Assert.IsType<InventoryDto>(createdResult.Value);
            Assert.Equal(20, returnedDto.Quantity);

            // double-check it's actually saved in the DB, not just returned
            Assert.Equal(1, await context.Inventories.CountAsync());
        }

        [Fact]
        public async Task Create_NegativeQuantity_ReturnsBadRequest()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var controller = new InventoryController(context);
            var dto = new InventoryCreateDto { ProductId = 1, Quantity = -5 };

            // Act
            var actionResult = await controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(actionResult.Result);
        }

        [Fact]
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
            Assert.IsType<NoContentResult>(result);
            var updated = await context.Inventories.FindAsync(inventory.InventoryId);
            Assert.Equal(50, updated!.Quantity);
        }

        [Fact]
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
            Assert.IsType<NoContentResult>(result);
            Assert.Equal(0, await context.Inventories.CountAsync());
        }

        [Fact]
        public async Task Delete_NonExistentRecord_ReturnsNotFound()
        {
            // Arrange
            await using var context = CreateInMemoryContext();
            var controller = new InventoryController(context);

            // Act
            var result = await controller.Delete(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}