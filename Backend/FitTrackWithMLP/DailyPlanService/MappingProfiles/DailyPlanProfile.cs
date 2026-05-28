using AutoMapper;
using DailyPlanService.Models;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;

namespace DailyPlanService.MappingProfiles
{
    public class DailyPlanProfile : Profile
    {
        public DailyPlanProfile()
        {
            CreateMap<CreateDailyPlanDto, DailyPlan>();
            CreateMap<CreatePlannedMealDto, PlannedMeal>();
            CreateMap<CreatePlannedIngredientDto, PlannedIngredient>();

            CreateMap<DailyPlan, DailyPlanDto>()
                .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src =>
                    src.Meals.Sum(m =>
                        m.Ingredients.Sum(i => (int)Math.Round((i.AmountG / 100.0) * i.Calories))
                    )));
            CreateMap<PlannedMeal, PlannedMealDto>()
                .ForMember(dest => dest.TotalCalories, opt => opt.MapFrom(src =>
                    src.Ingredients.Sum(i => (int)Math.Round((i.AmountG / 100.0) * i.Calories))));
            CreateMap<PlannedIngredient, PlannedMealIngredientDto>();

            CreateMap<EditDailyPlanDto, DailyPlan>();
            CreateMap<EditPlannedMealDto, PlannedMeal>();
            CreateMap<EditPlannedIngredientDto, PlannedIngredient>();
        }
    }
}
