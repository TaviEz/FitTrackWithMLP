namespace FitTrackWithMLP.Shared.Enums.Statuses
{
    public enum AddPlannedMealStatus
    {
        Created,
        NotFound,
        Failed
    }

    public sealed record AddPlannedMealResult(
        AddPlannedMealStatus Status,
        int? CreatedMealId = null)
    {
        public static AddPlannedMealResult Created(int mealId) =>
            new(AddPlannedMealStatus.Created, mealId);

        public static AddPlannedMealResult NotFound() =>
            new(AddPlannedMealStatus.NotFound);

        public static AddPlannedMealResult Failed() =>
            new(AddPlannedMealStatus.Failed);
    }
}