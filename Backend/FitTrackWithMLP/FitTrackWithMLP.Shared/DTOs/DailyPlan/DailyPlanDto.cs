namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class DailyPlanDto
    {
        public int TotalCalories { get; set; }
        public DateOnly TargetDate { get; set; }
        public List<PlannedMealDto> Meals { get; set; } = new();
    }
}
