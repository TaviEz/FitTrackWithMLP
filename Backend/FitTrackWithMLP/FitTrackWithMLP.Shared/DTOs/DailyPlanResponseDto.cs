namespace FitTrackWithMLP.Shared.DTOs
{
    public class DailyPlanResponseDto
    {
        public float TargetCalories { get; set; }
        public float ActualCalories { get; set; }
        public List<OptimizedMealPlanDto> Meals { get; set; } = new();
    }
}