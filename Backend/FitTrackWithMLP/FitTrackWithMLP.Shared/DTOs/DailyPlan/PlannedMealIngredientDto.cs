namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class PlannedMealIngredientDto
    {
        public int PlannedIngredientId { get; set; }
        public string Name { get; set; } = string.Empty;
        public double AmountG { get; set; }
    }
}
