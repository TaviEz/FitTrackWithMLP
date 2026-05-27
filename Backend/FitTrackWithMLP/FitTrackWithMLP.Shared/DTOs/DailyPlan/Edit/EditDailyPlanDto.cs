using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit
{
    public class EditDailyPlanDto
    {
        public List<EditPlannedMealDto> Meals { get; set; } = new();
    }
}
