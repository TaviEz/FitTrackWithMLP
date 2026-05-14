namespace FitTrackWithMLP.Shared.DTOs.DailyPlan
{
    public class CreateDailyPlanDto
    {
        public List<CreatePlannedMealDto> Meals { get; set; } = new();
    }
}
