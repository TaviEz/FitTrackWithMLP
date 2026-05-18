using FitTrackWithMLP.Shared.DTOs.DailyPlan;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums;

namespace FitTrackWithMLP.Shared.Logic
{
    public static class NutritionCalculator
    {
        public static string GetGroup(ActivityLevel activityLevel)
        {
            return activityLevel switch
            {
                ActivityLevel.Sedentary or ActivityLevel.LightlyActive => "G1",
                ActivityLevel.ModeratelyActive => "G2",
                ActivityLevel.VeryActive or ActivityLevel.ExtraActive => "G3",
                _ => throw new ArgumentOutOfRangeException(nameof(activityLevel), $"Not expected activity level value: {activityLevel}")
            };
        }

        public static NutritionTargetsDto GetDailyTargetsForGoal(int weight, int tdee, string activityGroup, GoalType goalType)
        {
            return goalType switch
            {
                GoalType.LoseFatAggressive => new NutritionTargetsDto
                {
                    Calories = (int)(tdee * 0.85),
                    Protein = (int)(weight * 1.4),
                    MinFat = (int)(weight * 0.7)
                },

                GoalType.LeanCutSlow => new NutritionTargetsDto
                {
                    Calories = (int)(tdee * 0.92),
                    Protein = (int)(weight * (activityGroup == "G1" ? 1.8 : activityGroup == "G2" ? 1.9 : 2.0)),
                    MinFat = (int)(weight * 0.8)
                },

                GoalType.MaintainFormTrained => new NutritionTargetsDto
                {
                    Calories = tdee,
                    Protein = (int)(weight * (activityGroup == "G1" ? 1.6 : activityGroup == "G2" ? 1.7 : 1.8)),
                    MinFat = (int)(weight * 0.8)
                },

                GoalType.MaintainFormHealthy => new NutritionTargetsDto
                {
                    Calories = tdee,
                    Protein = (int)(weight * 1.0),
                    MinFat = (int)(weight * 0.8)
                },

                GoalType.LeanBulk => new NutritionTargetsDto
                {
                    Calories = (int)(tdee * 1.08),
                    Protein = (int)(weight * (activityGroup == "G1" ? 1.6 : activityGroup == "G2" ? 1.7 : 1.8)),
                    MinFat = (int)(weight * 0.9)
                },

                GoalType.PowerBulk => new NutritionTargetsDto
                {
                    Calories = (int)(tdee * 1.15),
                    Protein = (int)(weight * 1.6),
                    MinFat = (int)(weight * 1.0)
                },

                _ => throw new ArgumentOutOfRangeException(nameof(goalType), $"Goal {goalType} not supported.")
            };
        }
    }
}
