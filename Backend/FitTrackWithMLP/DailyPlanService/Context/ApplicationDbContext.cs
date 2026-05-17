using DailyPlanService.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyPlanService.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<DailyPlan> DailyPlans { get; set; }
        public DbSet<PlannedMeal> PlannedMeals { get; set; }
        public DbSet<PlannedIngredient> PlannedMealIngredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DailyPlan>()
                .HasIndex(p => new { p.UserId, p.TargetDate })
                .IsUnique();
        }
    }
}
