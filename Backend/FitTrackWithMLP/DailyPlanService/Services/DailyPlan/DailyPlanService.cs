using AutoMapper;
using DailyPlanService.Context;
using DailyPlanService.Models;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Update;
using FitTrackWithMLP.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DailyPlanService.Services.DailyPlan
{
    public class DailyPlanService : IDailyPlanService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public DailyPlanService(
            ApplicationDbContext dbContext,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly targetDate)
        {
            var dailyPlan = await _dbContext.DailyPlans
                .Include(p => p.Meals)
                .ThenInclude(m => m.Ingredients)
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId) && p.TargetDate == targetDate);

            return dailyPlan is null ? null : _mapper.Map<DailyPlanDto>(dailyPlan);
        }

        public async Task<CreateDailyPlanStatus> CreateDailyPlanAsync(string userId, DateOnly targetDate, CreateDailyPlanDto dailyPlanDto)
        {
            var userGuid = Guid.Parse(userId);

            var exists = await _dbContext.DailyPlans.AnyAsync(p => p.UserId == userGuid && p.TargetDate == targetDate);

            if (exists)
            {
                return CreateDailyPlanStatus.AlreadyExists;
            }

            var dailyPlan = _mapper.Map<Models.DailyPlan>(dailyPlanDto);
            dailyPlan.UserId = userGuid;
            dailyPlan.TargetDate = targetDate;
            dailyPlan.CreatedAt = DateTime.UtcNow;
            dailyPlan.ModifiedAt = DateTime.UtcNow;

            _dbContext.DailyPlans.Add(dailyPlan);


            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? CreateDailyPlanStatus.Created : CreateDailyPlanStatus.Failed;
        }

        public async Task<ReplaceDailyPlanStatus> ReplaceDailyPlanAsync(string userId, int dailyPlanId, CreateDailyPlanDto dailyPlanDto)
        {
            var dailyPlan = await _dbContext.DailyPlans
                .Include(p => p.Meals)
                .ThenInclude(m => m.Ingredients)
                .Where(p => p.UserId == Guid.Parse(userId)
                    && p.DailyPlanId == dailyPlanId)
                .FirstOrDefaultAsync();

            if (dailyPlan == null)
                return ReplaceDailyPlanStatus.NotFound;

            dailyPlan.Meals.Clear();

            var newMeals = _mapper.Map<List<PlannedMeal>>(dailyPlanDto.Meals);

            foreach (var meal in newMeals)
            {
                dailyPlan.Meals.Add(meal);
            }

            dailyPlan.ModifiedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? ReplaceDailyPlanStatus.Replaced : ReplaceDailyPlanStatus.Failed;
        }

        public async Task<UpdateMealPlanStatus> UpdatePlannedMealAsync(string userId, int plannedMealId, UpdatePlannedMealDto updateDto)
        {
            var plannedMeal = await _dbContext.PlannedMeals
                .Include(m => m.Ingredients)
                .Include(m => m.DailyPlan)
                .Where(p => p.PlannedMealId == plannedMealId && p.DailyPlan.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (plannedMeal == null)
            {
                return UpdateMealPlanStatus.NotFound;
            }

            _mapper.Map(updateDto, plannedMeal);

            foreach (var ingredientDto in updateDto.Ingredients)
            {
                var existingIngredient = plannedMeal.Ingredients
                    .FirstOrDefault(i => i.PlannedIngredientId == ingredientDto.PlannedIngredientId);

                if (existingIngredient != null)
                {
                    existingIngredient.AmountG = ingredientDto.AmountG;
                }
            }

            plannedMeal.DailyPlan.ModifiedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync();

            return result > 0 ? UpdateMealPlanStatus.Updated : UpdateMealPlanStatus.Failed;
        }
    }
}
