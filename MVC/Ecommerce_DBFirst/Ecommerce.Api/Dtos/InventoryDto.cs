namespace Ecommerce.Api.Dtos
{
    public class InventoryDto
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class InventoryCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public class InventoryUpdateDto
    {
        public int Quantity { get; set; }
    }
}