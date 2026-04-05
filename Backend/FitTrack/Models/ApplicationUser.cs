using Microsoft.AspNetCore.Identity;

namespace FitTrack.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserDetails? UserDetails { get; set; }
    }
}
