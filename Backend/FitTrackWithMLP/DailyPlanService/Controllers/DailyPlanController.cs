using AutoMapper;
using DailyPlanService.Context;
using DailyPlanService.Models;
using FitTrackWithMLP.Shared.DTOs.DailyPlan;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Logic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DailyPlanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public DailyPlanController(
            ApplicationDbContext dbContext,
            IConfiguration configuration,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetDailyPlan([FromQuery] DateOnly dateTarget)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var dailyPlan = await _dbContext.DailyPlans
                .Include(p => p.Meals)
                .ThenInclude(m => m.Ingredients)
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId) && p.TargetDate == dateTarget);

            if (dailyPlan == null)
                return NotFound("No daily plan found for the specified date.");

            var dailyPlanDto = _mapper.Map<DailyPlanDto>(dailyPlan);

            return Ok(dailyPlanDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDailyPlan([FromBody] CreateDailyPlanDto dailyPlanDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token.");

            var dailyPlan = _mapper.Map<DailyPlan>(dailyPlanDto);
            dailyPlan.UserId = Guid.Parse(userId);
            dailyPlan.TargetDate = today;
            dailyPlan.CreatedAt = DateTime.UtcNow;
            dailyPlan.ModifiedAt = DateTime.UtcNow;

            _dbContext.DailyPlans.Add(dailyPlan);
            var result = await _dbContext.SaveChangesAsync();

            if (result > 0)
                return Ok(dailyPlan.DailyPlanId); 
            else
                return StatusCode(500, "Failed to save user details.");
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
