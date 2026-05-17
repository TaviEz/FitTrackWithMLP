using FitTrackWithMLP.Shared.Enums;

namespace DailyPlanService.Models
{
    public class PlannedMeal
    {
        public int PlannedMealId { get; set; }
        public int MealId { get; set; } // The ID from SQLite 'meals' table
        public string Title { get; set; } = string.Empty;
        public MealCategory Category { get; set; }

        // Foreign key to DailyPlan
        public int DailyPlanId { get; set; }
        // Navigation property
        public required List<PlannedIngredient> Ingredients { get; set; } = new();
    }
}
