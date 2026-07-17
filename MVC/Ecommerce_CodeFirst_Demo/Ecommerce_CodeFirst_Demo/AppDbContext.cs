using Microsoft.EntityFrameworkCore;
using Ecommerce_CodeFirst_Demo.Models;

namespace Ecommerce_CodeFirst_Demo
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Product> Products { get; set; }
    }
}
