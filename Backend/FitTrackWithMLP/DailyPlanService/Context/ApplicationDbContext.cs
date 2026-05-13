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
    }
}
