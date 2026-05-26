using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit
{
    public class EditDailyPlanDto
    {
        public int DailyPlanId { get; set; }
        public List<EditPlannedMealDto> Meals { get; set; } = new();
    }
}
