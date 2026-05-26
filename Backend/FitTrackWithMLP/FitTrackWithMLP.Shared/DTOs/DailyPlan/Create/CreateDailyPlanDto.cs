namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Create
{
    public class CreateDailyPlanDto
    {
        public List<CreatePlannedMealDto> Meals { get; set; } = new();
    }
}
