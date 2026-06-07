using Microsoft.AspNetCore.Identity;
using UserManagementService.Models;

namespace UserManagementService.Services.Authentication
{
    public class IdentityService: IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(UserManager<ApplicationUser> userManager,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ApplicationUser?> AuthenticateAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                _logger.LogWarning("Authentication failed: No user found with email {Email}", email);
                return null;
            }

            var validPassowrd = await _userManager.CheckPasswordAsync(user, password);
            if (!validPassowrd)
            {
                _logger.LogWarning("Authentication failed: Invalid password for email {Email}", email);
                return null;
            }

            return user;
        }

        public async Task<IdentityResult> RegisterAsync(string email, string password)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser is not null)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateEmail",
                    Description = "Email already in use"
                });
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email
            };

            return await _userManager.CreateAsync(user, password);
        }

        public async Task<ApplicationUser?> FindByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }
    }
}
