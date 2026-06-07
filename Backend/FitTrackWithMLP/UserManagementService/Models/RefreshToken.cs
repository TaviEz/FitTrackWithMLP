namespace UserManagementService.Models
{
    public class RefreshToken
    {
        public int RefreshTokenId { get; set; }
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ApplicationUser User { get; set; } = null!;
    }
}
