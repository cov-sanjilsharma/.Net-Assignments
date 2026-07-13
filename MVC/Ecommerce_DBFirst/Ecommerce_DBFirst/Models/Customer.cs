using System;
using System.Collections.Generic;

namespace Ecommerce_DBFirst.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string Email { get; set; } = null!;
}
