namespace DailyPlanService.Models
{
    public class DailyPlan
    {
        public int DailyPlanId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // Navigation property
        public required List<PlannedMeal> Meals { get; set; } = new();
    }
}
