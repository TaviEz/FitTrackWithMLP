using AutoMapper;
using DailyPlanService.Context;
using DailyPlanService.Models;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Update;
using FitTrackWithMLP.Shared.Enums.Statuses;
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

        public async Task<List<int>> GetLatestPlannedMealsAsync(string userId, int daysAgo = 7, int mealCap = 10)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var targetDate = today.AddDays(-daysAgo);

            var latestMealsIds = await _dbContext.PlannedMeals
                .Where(m => m.DailyPlan.UserId == Guid.Parse(userId)
                   && m.DailyPlan.TargetDate >= targetDate)
                .OrderByDescending(m => m.DailyPlan.TargetDate)
                .Take(mealCap)
                .Select(m => m.MealId)
                .ToListAsync();

            return latestMealsIds;
        }

        public async Task<AddPlannedMealResult> AddPlannedMealAsync(string userId, int dailyPlanId, AddPlannedMealItemDto addDto)
        {
            var dailyPlan = await _dbContext.DailyPlans
                .Where(p => p.DailyPlanId == dailyPlanId && p.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (dailyPlan == null)
            {
                return AddPlannedMealResult.NotFound();
            }

            var newMeal = _mapper.Map<PlannedMeal>(addDto);
            dailyPlan.Meals.Add(newMeal);
            dailyPlan.ModifiedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync();

            return result > 0
                ? AddPlannedMealResult.Created(newMeal.PlannedMealId)
                : AddPlannedMealResult.Failed();
        }

        public async Task<UpdatePlannedMealTitleStatus> UpdatePlannedMealTitleAsync(string userId, int plannedMealId, string title)
        {
            var plannedMeal = await _dbContext.PlannedMeals
                .Include(m => m.DailyPlan)
                .Where(m => m.PlannedMealId == plannedMealId && m.DailyPlan.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (plannedMeal == null)
            {
                return UpdatePlannedMealTitleStatus.NotFound;
            }

            plannedMeal.Title = title;
            plannedMeal.DailyPlan.ModifiedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? UpdatePlannedMealTitleStatus.Success : UpdatePlannedMealTitleStatus.Failed;
        }

        public async Task<AddPlannedIngredientStatus> AddPlannedIngredientAsnyc(string userId, int plannedMealId, CreatePlannedIngredientDto addDto)
        {
            var plannedMeal = await _dbContext.PlannedMeals
                .Include(m => m.DailyPlan)
                .Where(m => m.PlannedMealId == plannedMealId && m.DailyPlan.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (plannedMeal == null)
            {
                return AddPlannedIngredientStatus.NotFound;
            }

            var existingIngredient = await _dbContext.PlannedMealIngredients
                .Where(i => i.PlannedMealId == plannedMealId && i.FoodId == addDto.FoodId)
                .FirstOrDefaultAsync();

            if (existingIngredient != null)
            {
                return AddPlannedIngredientStatus.AlreadyExists;
            }


            var newIngredient = _mapper.Map<PlannedIngredient>(addDto);
            plannedMeal.Ingredients.Add(newIngredient);
            plannedMeal.DailyPlan.ModifiedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync();

            return result > 0 ? AddPlannedIngredientStatus.Created : AddPlannedIngredientStatus.Failed;
        }
        
        public async Task<UpdatePlannedIngredientStatus> UpdatePlannedIngredientAsync(string userId, int plannedIngredientId, UpdatePlannedIngredientDto updateDto)
        {
            var plannedIngredient = await _dbContext.PlannedMealIngredients
                .Include(i => i.PlannedMeal)
                .ThenInclude(m => m.DailyPlan)
                .Where(i => i.PlannedIngredientId == plannedIngredientId && i.PlannedMeal.DailyPlan.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (plannedIngredient == null)
            {
                return UpdatePlannedIngredientStatus.NotFound;
            }

            plannedIngredient.PlannedMeal.DailyPlan.ModifiedAt = DateTime.UtcNow;
            plannedIngredient.AmountG = updateDto.AmountG;

            var result = await _dbContext.SaveChangesAsync();

            return result > 0 ? UpdatePlannedIngredientStatus.Updated : UpdatePlannedIngredientStatus.Failed;
        }

        public async Task<DeletePlannedIngredientStatus> DeletePlannedIngredientAsync(string userId, int plannedMealId, int plannedIngredientId)
        {
            var plannedIngredient = await _dbContext.PlannedMealIngredients
                .Include(i => i.PlannedMeal)
                .ThenInclude(m => m.DailyPlan)
                .Where(i => i.PlannedIngredientId == plannedIngredientId
                    && i.PlannedMealId == plannedMealId
                    && i.PlannedMeal.DailyPlan.UserId == Guid.Parse(userId))
                .FirstOrDefaultAsync();

            if (plannedIngredient == null)
            {
                return DeletePlannedIngredientStatus.NotFound;
            }

            _dbContext.PlannedMealIngredients.Remove(plannedIngredient);
            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? DeletePlannedIngredientStatus.Deleted : DeletePlannedIngredientStatus.Failed;
        }
    }
}
