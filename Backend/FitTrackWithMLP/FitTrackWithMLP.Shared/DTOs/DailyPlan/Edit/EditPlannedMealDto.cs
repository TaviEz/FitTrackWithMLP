using FitTrackWithMLP.Shared.Enums;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit
{
    public class EditPlannedMealDto
    {
        public string Title { get; set; } = string.Empty;
        public MealCategory Category { get; set; }
        public List<EditPlannedIngredientDto> Ingredients { get; set; } = new();
    }
}
