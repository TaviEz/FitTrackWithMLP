using FitTrackWithMLP.Shared.DTOs;
using FitTrackWithMLP.Shared.Enums;
using FitTrackWithMLP.Shared.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDailyPlan(UserPhysiqueDto userPhysiqueDto)
        {
            if (!Enum.TryParse(userPhysiqueDto.ActivityLevel, true, out ActivityLevel userActivityLevel))
                return BadRequest("Invalid activity level");

            var activityGroup = NutritionCalculator.GetGroup(userActivityLevel);
            // TODO: add goal type inside the userPhysiqueDto
            var goalType = GoalType.MaintainFormTrained;
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userPhysiqueDto, activityGroup, goalType);

            var handler = new HttpClientHandler
            {
                Proxy = null,
                UseProxy = false
            };
            using var client = new HttpClient(handler);
            var baseUrl = _configuration["Base:Url"] ?? "";

            if (string.IsNullOrEmpty(baseUrl))
                return StatusCode(500, "Optimizer service URL is not configured.");

            var optimizerRequest = new OptimizedRequestDto
            {
                Calories = dailyTargets.Calories,
                Protein = dailyTargets.Protein,
                MinFat = dailyTargets.MinFat,
                MealsComplexity = "Standard"
            };

            var optimizeUrl = baseUrl + "/optimize";
            HttpResponseMessage response = await client.PostAsJsonAsync(optimizeUrl, optimizerRequest);

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Optimizer service failed.");
            }

            var optimizedPlan = await response.Content.ReadFromJsonAsync<List<OptimizedMealPlanDto>>();

            return Ok(optimizedPlan);
        }
    }
}
