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
        private readonly IConfiguration _configuration;

        public DailyPlanController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDailyPlan([FromBody] CreateDailyPlanDto dailyPlanDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Ok();
        }

        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDailyPlan([FromBody] UserPhysiqueDto userPhysiqueDto)
        {
            if (!Enum.TryParse(userPhysiqueDto.ActivityLevel, true, out ActivityLevel userActivityLevel))
                return BadRequest("Invalid activity level");

            if (!Enum.TryParse(userPhysiqueDto.GoalType, true, out GoalType userGoalType))
                return BadRequest("Invalid goal type");

            if (userPhysiqueDto.MealsComplexity != "Standard" && userPhysiqueDto.MealsComplexity != "Simple")
                return BadRequest("Invalid meals complexity. Allowed values are 'Standard' or 'Simple'.");

            var activityGroup = NutritionCalculator.GetGroup(userActivityLevel);
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userPhysiqueDto, activityGroup, userGoalType);

            var handler = new HttpClientHandler
            {
                Proxy = null,
                UseProxy = false
            };
            using var client = new HttpClient(handler);
            var baseUrl = _configuration["Optimizer:Url"];

            if (string.IsNullOrEmpty(baseUrl))
                return StatusCode(500, "Optimizer service URL is not configured.");

            var optimizerRequest = new OptimizedRequestDto
            {
                Calories = dailyTargets.Calories,
                Protein = dailyTargets.Protein,
                MinFat = dailyTargets.MinFat,
                MealsComplexity = userPhysiqueDto.MealsComplexity
            };

            var optimizeUrl = baseUrl + "/optimize";
            HttpResponseMessage response = await client.PostAsJsonAsync(optimizeUrl, optimizerRequest);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Optimizer service failed.");
            }

            var optimizedPlan = await response.Content.ReadFromJsonAsync<List<OptimizedMealPlanDto>>();

            if (optimizedPlan == null)
                return StatusCode(500, "Failed to parse optimizer response.");

            var actualCalories = optimizedPlan.Sum(m => m.Calories);

            var result = new DailyPlanResponseDto
            {
                TargetCalories = dailyTargets.Calories,
                ActualCalories = actualCalories,
                Meals = optimizedPlan
            };

            return Ok(result);
        }
    }
}
