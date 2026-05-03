using Microsoft.AspNetCore.Identity;

namespace UserManagementService.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserDetails? UserDetails { get; set; }
    }
}
