using Ecommerce_DBFirst.Models;
namespace Ecommerce_DBFirst.Dtos
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        public int StockQuantit { get; set; }
        public int CategoryId { get; set; }

        public string ?CategoryName{ get; set; }
    }
}
