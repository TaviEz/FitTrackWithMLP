namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Update
{
    public class UpdatePlannedMealDto
    {
        public string Title { get; set; } = string.Empty;
        public List<UpdatePlannedIngredientDto> Ingredients { get; set; } = new();
    }
}
