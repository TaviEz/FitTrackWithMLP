namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Generate
{
    public class GeneratedDailyPlanDto
    {
        public float TargetCalories { get; set; }
        public float ActualCalories { get; set; }
        public List<OptimizedMealPlanDto> Meals { get; set; } = new();
    }
}