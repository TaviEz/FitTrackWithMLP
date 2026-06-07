using UserManagementService.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(au => au.UserDetails)
                .WithOne(u => u.User)
                .HasForeignKey<UserDetails>(u => u.UserId);

            modelBuilder.Entity<UserDetails>()
                .Property(u => u.Gender)
                .HasConversion<string>();

            modelBuilder.Entity<UserDetails>()
                .Property(u => u.ActivityLevel)
                .HasConversion<string>();

            modelBuilder.Entity<UserDetails>()
                .Property(u => u.GoalType)
                .HasConversion<string>();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId);
        }

        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
