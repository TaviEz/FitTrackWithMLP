using AutoMapper;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums.Statuses;
using FitTrackWithMLP.Shared.Logic;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Context;

namespace UserManagementService.Services.UserDetails
{
    public class UserDetailsService : IUserDetailsService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<UserDetailsService> _logger;

        public UserDetailsService(
            ApplicationDbContext dbContext, 
            IMapper mapper,
            ILogger<UserDetailsService> logger)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<UserDetailsDto?> GetUserDetailsAsync(string userId)
        {
            var userDetails = await _dbContext.UserDetails
                .FirstOrDefaultAsync(ud => ud.UserId == userId);

            return userDetails is null ? null : _mapper.Map<UserDetailsDto>(userDetails);
        }

        public async Task<UserDetailsOperationResult> UpsertUserDetailsAsync(string userId, UserDetailsDto userDto)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(ud => ud.UserDetails)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("StoreUserDetails failed. User not found for userId: {UserId}", userId);
                return UserDetailsOperationResult.UserNotFound();
            }

            // get the daily targets for the user
            var activityGroup = NutritionCalculator.GetGroup(userDto.ActivityLevel);
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(
                userDto.Weight, userDto.Tdee, activityGroup, userDto.GoalType
            );

            if (user.UserDetails == null)
            {
                var newUserDetails = _mapper.Map<Models.UserDetails>(userDto);
                newUserDetails.UserId = userId;
                newUserDetails.Date = DateTime.UtcNow;
                newUserDetails.TargetCalories = dailyTargets.Calories;

                _dbContext.UserDetails.Add(newUserDetails);
            }
            else
            {
                _mapper.Map(userDto, user.UserDetails);
                user.UserDetails.Date = DateTime.UtcNow;
                user.UserDetails.TargetCalories = dailyTargets.Calories;
            }

            var result = await _dbContext.SaveChangesAsync();
            return result > 0
                ? UserDetailsOperationResult.Success()
                : UserDetailsOperationResult.Failed("Failed to save user details.");
        }
    }
}
