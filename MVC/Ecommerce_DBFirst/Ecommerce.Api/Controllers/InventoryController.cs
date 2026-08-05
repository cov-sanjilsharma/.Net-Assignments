using Ecommerce.Api.Data;
using Ecommerce.Api.Dtos;
using Ecommerce.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryDbContext _context;

        public InventoryController(InventoryDbContext context)
        {
            _context = context;
        }

        // GET: api/inventory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryDto>>> GetAll()
        {
            var items = await _context.Inventories
                .Select(i => new InventoryDto
                {
                    InventoryId = i.InventoryId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    LastUpdated = i.LastUpdated
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET: api/inventory/product/5
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<InventoryDto>> GetByProductId(int productId)
        {
            var item = await _context.Inventories
                .Where(i => i.ProductId == productId)
                .Select(i => new InventoryDto
                {
                    InventoryId = i.InventoryId,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    LastUpdated = i.LastUpdated
                })
                .FirstOrDefaultAsync();

            if (item == null)
                return NotFound(new { message = $"No inventory record found for product {productId}." });

            return Ok(item);
        }

        // POST: api/inventory
        [HttpPost]
        public async Task<ActionResult<InventoryDto>> Create([FromBody] InventoryCreateDto dto)
        {
            if (dto.Quantity < 0)
                return BadRequest(new { message = "Quantity cannot be negative." });

            var entity = new Inventory
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                LastUpdated = DateTime.Now
            };

            _context.Inventories.Add(entity);
            await _context.SaveChangesAsync();

            var result = new InventoryDto
            {
                InventoryId = entity.InventoryId,
                ProductId = entity.ProductId,
                Quantity = entity.Quantity,
                LastUpdated = entity.LastUpdated
            };

            return CreatedAtAction(nameof(GetByProductId), new { productId = entity.ProductId }, result);
        }

        // PUT: api/inventory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody] InventoryUpdateDto dto)
        {
            if (dto.Quantity < 0)
                return BadRequest(new { message = "Quantity cannot be negative." });

            var entity = await _context.Inventories.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Inventory record {id} not found." });

            entity.Quantity = dto.Quantity;
            entity.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/inventory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Inventories.FindAsync(id);
            if (entity == null)
                return NotFound(new { message = $"Inventory record {id} not found." });

            _context.Inventories.Remove(entity);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}