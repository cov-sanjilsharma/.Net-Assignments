using Microsoft.AspNetCore.Identity;

namespace Ecommerce_DBFirst.Models
{
    // Extends IdentityUser with any extra fields you want to track.
    // Id, UserName, Email, PasswordHash are already provided by IdentityUser.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
    }
}