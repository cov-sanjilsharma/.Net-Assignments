using System;
using System.Collections.Generic;

namespace Ecommerce_DBFirst.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;
    public virtual ICollection<Products> Products { get; set; } = new List<Products>();
}
