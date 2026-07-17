using System;
using System.Collections.Generic;

namespace Ecommerce_DBFirst.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public string CustomerId { get; set; } = null!;

    public DateTime OrderDate { get; set; }

    public int TotalAmount { get; set; }
}
