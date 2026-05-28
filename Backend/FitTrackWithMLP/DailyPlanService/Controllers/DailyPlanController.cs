using DailyPlanService.Services.DailyPlan;
using DailyPlanService.Services.MealOptimzer;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Edit;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Generate;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums;
using FitTrackWithMLP.Shared.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DailyPlanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyPlanController : ControllerBase
    {
        private readonly IDailyPlanService _dailyPlanService;
        private readonly IMealOptimizerClient _mealOptimizerClient;
        private readonly ILogger<DailyPlanController> _logger;

        public DailyPlanController(
            IDailyPlanService dailyPlanService,
            IMealOptimizerClient mealOptimizerClient,
            ILogger<DailyPlanController> logger)
        {
            _dailyPlanService = dailyPlanService;
            _mealOptimizerClient = mealOptimizerClient;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDailyPlan([FromQuery] DateOnly dateTarget)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var dailyPlanDto = await _dailyPlanService.GetDailyPlanAsync(userId, dateTarget);

            if (dailyPlanDto is null)
            {
                _logger.LogWarning("No daily plan found for user {UserId} on {Date}", userId, dateTarget);
                return Ok(null);
            }

            _logger.LogInformation("Daily plan retrieved for user {UserId} on {Date}", userId, dateTarget);
            return Ok(dailyPlanDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDailyPlan([FromBody] CreateDailyPlanDto dailyPlanDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            if (dailyPlanDto.Meals is null || dailyPlanDto.Meals.Count == 0)
            {
                _logger.LogWarning("Attempt to create a daily plan with no meals for user {UserId}", userId);
                return BadRequest("At least one meal is required.");
            }

            var result = await _dailyPlanService.CreateDailyPlanAsync(userId, dailyPlanDto);

            switch (result)
            {
                case CreateDailyPlanStatus.Created:
                    _logger.LogInformation("Daily plan created for user {UserId}", userId);
                    return Created();
                case CreateDailyPlanStatus.AlreadyExists:
                    _logger.LogWarning("Daily plan already exists for user {UserId} today", userId);
                    return Conflict("A daily plan already exists for today.");
                default:
                    _logger.LogError("Failed to create daily plan for user {UserId}", userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut("{dailyPlanId:int}")]
        public async Task<IActionResult> ReplaceDailyPlan(
            [FromRoute] int dailyPlanId, 
            [FromBody] EditDailyPlanDto dailyPlanDto)    
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var result = await _dailyPlanService.ReplaceDailyPlanAsync(userId, dailyPlanId, dailyPlanDto);

            switch (result)
            {
                case ReplaceDailyPlanStatus.Replaced:
                    _logger.LogInformation("Daily plan {DailyPlanId} replaced for user {UserId}", dailyPlanId, userId);
                    return Ok();
                case ReplaceDailyPlanStatus.NotFound:
                    _logger.LogWarning("Daily plan {DailyPlanId} not found for user {UserId}", dailyPlanId, userId);
                    return NotFound();
                default:
                    _logger.LogError("Failed to replace daily plan {DailyPlanId} for user {UserId}", dailyPlanId, userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDailyPlan([FromBody] UserPhysiqueDto userPhysiqueDto)
        {
            var activityGroup = NutritionCalculator.GetGroup(userPhysiqueDto.ActivityLevel);
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(
                userPhysiqueDto.Weight, userPhysiqueDto.Tdee, activityGroup, userPhysiqueDto.GoalType
            );

            var optimizerRequest = new OptimizedRequestDto
            {
                Calories = dailyTargets.Calories,
                Protein = dailyTargets.Protein,
                MinFat = dailyTargets.MinFat,
                MealsComplexity = userPhysiqueDto.MealsComplexity.ToString()
            };

            var optimizedPlan = await _mealOptimizerClient.OptimizeAsync(optimizerRequest);

            if (optimizedPlan == null)
            {
                _logger.LogError("Meal optimizer failed to generate a plan for user with activity level {ActivityLevel} and goal {GoalType}",
                    userPhysiqueDto.ActivityLevel, userPhysiqueDto.GoalType);
                return StatusCode(500, "Failed to parse optimizer response.");
            }

            var actualCalories = optimizedPlan.Sum(m => m.Calories);

            var result = new GeneratedDailyPlanDto
            {
                TargetCalories = dailyTargets.Calories,
                ActualCalories = actualCalories,
                Meals = optimizedPlan
            };

            _logger.LogInformation("Generated daily plan for user with activity level {ActivityLevel} and goal {GoalType}. Target calories: {TargetCalories}, Actual calories: {ActualCalories}",
                userPhysiqueDto.ActivityLevel, userPhysiqueDto.GoalType, result.TargetCalories, result.ActualCalories);
            return Ok(result);
        }
    }
}
