using Microsoft.EntityFrameworkCore;
using UserManagementService.Context;
using UserManagementService.Models;

namespace UserManagementService.Services.Authentication
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ApplicationDbContext _dbContext;

        public RefreshTokenService(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public async Task<string> GenerateAndStoreAsync(string userId)
        {
            var randomBytes = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            var token = Convert.ToBase64String(randomBytes);

            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = token,
                Expires = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.RefreshTokens.Add(refreshToken);
            await _dbContext.SaveChangesAsync();

            return token;
        }

        public async Task<RefreshToken?> ValidateAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .Where(rt => rt.Token == token && rt.Expires > DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteAsync(string token)
        {
            var refreshToken = await _dbContext.RefreshTokens
                .Where(rt => rt.Token == token)
                .FirstOrDefaultAsync();

            if (refreshToken is not null)
            {
                _dbContext.RefreshTokens.Remove(refreshToken);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
