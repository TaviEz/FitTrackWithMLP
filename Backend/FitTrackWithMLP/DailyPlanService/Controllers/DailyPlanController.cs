using DailyPlanService.Services.DailyPlan;
using DailyPlanService.Services.MealOptimzer;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Create;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Generate;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Get;
using FitTrackWithMLP.Shared.DTOs.DailyPlan.Update;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums.Statuses;
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
        public async Task<IActionResult> GetDailyPlan([FromQuery] DateOnly targetDate)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var dailyPlanDto = await _dailyPlanService.GetDailyPlanAsync(userId, targetDate);

            if (dailyPlanDto is null)
            {
                _logger.LogWarning("No daily plan found for user {UserId} on {Date}", userId, targetDate);
                return Ok(null);
            }

            _logger.LogInformation("Daily plan retrieved for user {UserId} on {Date}", userId, targetDate);
            return Ok(dailyPlanDto);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateDailyPlan(
            [FromQuery] DateOnly targetDate, 
            [FromBody] CreateDailyPlanDto dailyPlanDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            if (dailyPlanDto.Meals is null)
            {
                _logger.LogWarning("Attempt to create a daily plan with no meals for user {UserId}", userId);
                return BadRequest("At least one meal is required.");
            }

            var result = await _dailyPlanService.CreateDailyPlanAsync(userId, targetDate, dailyPlanDto);

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
            [FromBody] CreateDailyPlanDto dailyPlanDto)    
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var activityGroup = NutritionCalculator.GetGroup(userPhysiqueDto.ActivityLevel);
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(
                userPhysiqueDto.Weight, userPhysiqueDto.Tdee, activityGroup, userPhysiqueDto.GoalType
            );

            var excludedMealIds = await _dailyPlanService.GetLatestPlannedMealsAsync(userId);

            var optimizerRequest = new OptimizedRequestDto
            {
                Calories = dailyTargets.Calories,
                Protein = dailyTargets.Protein,
                MinFat = dailyTargets.MinFat,
                MealsComplexity = userPhysiqueDto.MealsComplexity.ToString(),
                ExcludedMealIds = excludedMealIds
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

        [Authorize]
        [HttpPut("meal/{plannedMealId:int}/title")]
        public async Task<IActionResult> UpdatePlannedMealTitle(
            [FromRoute] int plannedMealId, 
            [FromBody] UpdatePlannedMealTitleDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var result = await _dailyPlanService.UpdatePlannedMealTitleAsync(userId, plannedMealId, updateDto.Title);

            switch (result)
            {
                case UpdatePlannedMealTitleStatus.Success:
                    _logger.LogInformation("Planned meal {PlannedMealId} title updated for user {UserId}", plannedMealId, userId);
                    return Ok();
                case UpdatePlannedMealTitleStatus.NotFound:
                    _logger.LogWarning("Planned meal {PlannedMealId} not found for user {UserId}", plannedMealId, userId);
                    return NotFound();
                default:
                    _logger.LogError("Failed to update title for planned meal {PlannedMealId} for user {UserId}", plannedMealId, userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPost("meal/{plannedMealId:int}/ingredient")]
        public async Task<IActionResult> AddPlannedIngredient(
            [FromRoute] int plannedMealId,
            [FromBody] CreatePlannedIngredientDto addDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var result = await _dailyPlanService.AddPlannedIngredientAsnyc(userId, plannedMealId, addDto);

            switch (result)
            {
                case AddIngredientRowStatus.Created:
                    _logger.LogInformation("Planned ingredient added to meal {PlannedMealId} for user {UserId}", plannedMealId, userId);
                    return Ok();
                case AddIngredientRowStatus.NotFound:
                    _logger.LogWarning("Failed to add planned ingredient to meal {PlannedMealId} for user {UserId} - meal not found", plannedMealId, userId);
                    return NotFound();
                case AddIngredientRowStatus.AlreadyExists:
                    _logger.LogWarning("Planned ingredient already exists in meal {PlannedMealId} for user {UserId}", plannedMealId, userId);
                    return Conflict();
                default:
                    _logger.LogError("Failed to add planned ingredient to meal {PlannedMealId} for user {UserId}", plannedMealId, userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpPut("ingredient/{plannedIngredientId:int}")]
        public async Task<IActionResult> UpdatePlannedIngredient(
            [FromRoute] int plannedIngredientId,
            [FromBody] UpdatePlannedIngredientDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var result = await _dailyPlanService.UpdatePlannedIngredientAsync(userId, plannedIngredientId, updateDto);

            switch (result)
            {
                case UpdatePlannedIngredientStatus.Updated:
                    _logger.LogInformation("Planned ingredient {PlannedIngredientId} updated for user {UserId}", plannedIngredientId, userId);
                    return Ok();
                case UpdatePlannedIngredientStatus.NotFound:
                    _logger.LogWarning("Failed to update planned ingredient {PlannedIngredientId} for user {UserId}", plannedIngredientId, userId);
                    return NotFound();
                default:
                    _logger.LogError("Failed to update planned ingredient {PlannedIngredientId} for user {UserId}", plannedIngredientId, userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [Authorize]
        [HttpDelete("ingredient")]
        public async Task<IActionResult> DeletePlannedIngredient(
            [FromQuery] int plannedMealId,
            [FromQuery] int plannedIngredientId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var result = await _dailyPlanService.DeletePlannedIngredientAsync(userId, plannedMealId, plannedIngredientId);

            switch (result)
            {
                case DeletePlannedIngredientStatus.Deleted:
                    _logger.LogInformation("Planned ingredient {PlannedIngredientId} deleted from meal {PlannedMealId} for user {UserId}",
                        plannedIngredientId, plannedMealId, userId);
                    break;
                case DeletePlannedIngredientStatus.NotFound:
                    _logger.LogWarning("Planned ingredient {PlannedIngredientId} not found in meal {PlannedMealId} for user {UserId}",
                        plannedIngredientId, plannedMealId, userId);
                    return NotFound();
                default:
                    _logger.LogError("Failed to delete planned ingredient {PlannedIngredientId} from meal {PlannedMealId} for user {UserId}",
                        plannedIngredientId, plannedMealId, userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("ingredient/search")]
        public async Task<IActionResult> GetIngredientOptions(
            [FromQuery] string query)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                return Unauthorized("User ID not found in token.");

            var ingredientOptions = await _mealOptimizerClient.GetIngredientOptionsAsync(query);
            if (ingredientOptions == null || !ingredientOptions.Any())
            {
                _logger.LogInformation("No ingredient options found for query '{Query}' for user {UserId}", query, userId);
                return Ok(Array.Empty<IngredientOptionDto>());
            }

            _logger.LogInformation("{Count} ingredient options found for query '{Query}' for user {UserId}", ingredientOptions.Count(), query, userId);
            return Ok(ingredientOptions);
        }
    }
}
