using AutoMapper;
using DailyPlanService.Models;
using FitTrackWithMLP.Shared.DTOs.DailyPlan;

namespace DailyPlanService.MappingProfiles
{
    public class DailyPlanProfile : Profile
    {
        public DailyPlanProfile()
        {
            CreateMap<CreateDailyPlanDto, DailyPlan>();
            CreateMap<CreatePlannedMealDto, PlannedMeal>();
            CreateMap<CreatePlannedIngredientDto, PlannedIngredient>();
        }
    }
}
