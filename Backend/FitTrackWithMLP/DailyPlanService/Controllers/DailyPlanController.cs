using AutoMapper;
using DailyPlanService.Models;
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
        private readonly IMapper _mapper;

        public DailyPlanController(
            IConfiguration configuration,
            IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDailyPlan([FromBody] CreateDailyPlanDto dailyPlanDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var dailyPlan = _mapper.Map<DailyPlan>(dailyPlanDto);
            dailyPlan.UserId = Guid.Parse(userId);
            dailyPlan.CreatedAt = DateTime.UtcNow;
            dailyPlan.ModifiedAt = DateTime.UtcNow;

            return Ok(dailyPlan);
        }

        [Authorize]
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateDailyPlan([FromBody] UserPhysiqueDto userPhysiqueDto)
        {
            var activityGroup = NutritionCalculator.GetGroup(userPhysiqueDto.ActivityLevel);
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userPhysiqueDto, activityGroup);

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
                MealsComplexity = userPhysiqueDto.MealsComplexity.ToString()
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
