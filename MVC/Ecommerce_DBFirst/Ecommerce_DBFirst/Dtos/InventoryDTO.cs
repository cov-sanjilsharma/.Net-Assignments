namespace Ecommerce_DBFirst.Dtos
{
    public class InventoryDTO
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}