using System;
using System.Collections.Generic;

namespace Ecommerce.Api.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int StockQuantit { get; set; }

    public int CategoryId { get; set; }

    public virtual ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
}
