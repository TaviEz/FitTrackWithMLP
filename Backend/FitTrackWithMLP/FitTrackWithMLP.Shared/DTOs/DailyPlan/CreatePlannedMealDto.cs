using FitTrackWithMLP.Shared.Enums;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class CreatePlannedMealDto
    {
        public int MealId { get; set; } // SQLite ID
        public string Title { get; set; } = string.Empty;
        public MealCategory Category { get; set; }
        public List<CreatePlannedIngredientDto> Ingredients { get; set; } = new();
    }
}
