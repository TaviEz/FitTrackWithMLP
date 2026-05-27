namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    // TODO: organize the dtos at this level in folders similar to create and edit dtos
    public class DailyPlanDto
    {
        public int DailyPlanId { get; set; }
        public int TotalCalories { get; set; }
        public List<PlannedMealDto> Meals { get; set; } = new();
    }
}
