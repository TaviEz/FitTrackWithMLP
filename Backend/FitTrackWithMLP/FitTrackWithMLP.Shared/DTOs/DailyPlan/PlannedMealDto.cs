namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class PlannedMealDto
    {
        public int PlannedMealId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<PlannedMealIngredientDto> Ingredients { get; set; } = new();
        // Total calories are computed on the fly via AutoMapper
        public int TotalCalories { get; set; }
    }
}
