using System;
using System.Collections.Generic;

namespace Ecommerce_DBFirst.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;
}
