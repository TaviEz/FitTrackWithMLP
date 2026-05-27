using AutoMapper;
using DailyPlanService.Context;
using FitTrackWithMLP.Shared.DTOs.DailyPlan;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit;
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
        public async Task<DailyPlanDto?> GetDailyPlanAsync(string userId, DateOnly dateTarget)
        {
            var dailyPlan = await _dbContext.DailyPlans
                .Include(p => p.Meals)
                .ThenInclude(m => m.Ingredients)
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId) && p.TargetDate == dateTarget);

            return dailyPlan is null ? null : _mapper.Map<DailyPlanDto>(dailyPlan);
        }

        public async Task<CreateDailyPlanStatus> CreateDailyPlanAsync(string userId, CreateDailyPlanDto dailyPlanDto)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var userGuid = Guid.Parse(userId);

            var exists = await _dbContext.DailyPlans.AnyAsync(p => p.UserId == userGuid && p.TargetDate == today);

            if (exists)
            {
                return CreateDailyPlanStatus.AlreadyExists;
            }

            var dailyPlan = _mapper.Map<Models.DailyPlan>(dailyPlanDto);
            dailyPlan.UserId = userGuid;
            dailyPlan.TargetDate = today;
            dailyPlan.CreatedAt = DateTime.UtcNow;
            dailyPlan.ModifiedAt = DateTime.UtcNow;

            _dbContext.DailyPlans.Add(dailyPlan);


            var result = await _dbContext.SaveChangesAsync();
            return result > 0 ? CreateDailyPlanStatus.Created : CreateDailyPlanStatus.Failed;
        }

        // TODO: use dailyPlan.Meals.Clear and map the new meals to dailyplandto.meals
        public async Task<bool> ReplaceDailyPlanAsync(string userId, int dailyPlanId, EditDailyPlanDto dailyPlanDto)
        {
            var dailyPlan = await _dbContext.DailyPlans
                .Include(p => p.Meals)
                .ThenInclude(m => m.Ingredients)
                .Where(p => p.UserId == Guid.Parse(userId)
                    && p.DailyPlanId == dailyPlanId)
                .FirstOrDefaultAsync();

            if (dailyPlan == null)
                return false;

            _mapper.Map(dailyPlanDto, dailyPlan);
            dailyPlan.ModifiedAt = DateTime.UtcNow;

            var result = await _dbContext.SaveChangesAsync();
            return result > 0;
        }
    }
}
