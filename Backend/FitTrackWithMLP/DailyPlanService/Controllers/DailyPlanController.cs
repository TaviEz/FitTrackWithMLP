using AutoMapper;
using DailyPlanService.Context;
using DailyPlanService.Services.DailyPlan;
using DailyPlanService.Services.MealOptimzer;
using FitTrackWithMLP.Shared.DTOs.DailyPlan;
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

        public DailyPlanController(
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            IMapper mapper,
            IDailyPlanService dailyPlanService,
            IMealOptimizerClient mealOptimizerClient)
        {
            _dailyPlanService = dailyPlanService;
            _mealOptimizerClient = mealOptimizerClient;
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
                return NotFound();
            }

            return Ok(dailyPlanDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDailyPlan([FromBody] CreateDailyPlanDto dailyPlanDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            if (dailyPlanDto.Meals is null || dailyPlanDto.Meals.Count == 0)
            {
                return BadRequest("At least one meal is required.");
            }

            var result = await _dailyPlanService.CreateDailyPlanAsync(userId, dailyPlanDto);

            return result switch
            {
                CreateDailyPlanStatus.Created => Ok(),
                CreateDailyPlanStatus.AlreadyExists => Conflict("A daily plan already exists for today."),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
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
                return StatusCode(500, "Failed to parse optimizer response.");

            var actualCalories = optimizedPlan.Sum(m => m.Calories);

            var result = new GeneratedDailyPlanDto
            {
                TargetCalories = dailyTargets.Calories,
                ActualCalories = actualCalories,
                Meals = optimizedPlan
            };

            return Ok(result);
        }
    }
}
