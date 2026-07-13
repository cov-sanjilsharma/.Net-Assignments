using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_DBFirst.Models
{
    // IdentityDbContext<ApplicationUser> already defines DbSets for
    // Users, Roles, UserRoles, UserClaims, UserLogins, etc.
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}