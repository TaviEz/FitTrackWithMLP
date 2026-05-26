namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class DailyPlanDto
    {
        public int DailyPlanId { get; set; }
        public int TotalCalories { get; set; }
        public List<PlannedMealDto> Meals { get; set; } = new();
    }
}
