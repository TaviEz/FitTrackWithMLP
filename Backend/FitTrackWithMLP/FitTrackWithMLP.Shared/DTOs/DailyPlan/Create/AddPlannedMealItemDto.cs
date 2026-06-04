using FitTrackWithMLP.Shared.Enums.Lookups;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitTrackWithMLP.Shared.DTOs.DailyPlan.Create
{
    public class AddPlannedMealItemDto
    {
        public string Title { get; set; } = string.Empty;
        public MealCategory Category { get; set; }
    }
}
