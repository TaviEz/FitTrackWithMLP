using FitTrackWithMLP.Shared.DTOs;
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
        [Authorize]
        [HttpPost("generate")]
        public IActionResult GenerateDailyPlan(UserDetailsDto userDetailsDto)
        {
            if (!Enum.TryParse(userDetailsDto.ActivityLevel, true, out ActivityLevel userActivityLevel))
                return BadRequest("Invalid activity level");

            var activityGroup = NutritionCalculator.GetGroup(userActivityLevel);
            // TODO: add goal type inside the userDetailsDto
            var goalType = GoalType.MaintainFormTrained;
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userDetailsDto, activityGroup, goalType);

            return Ok(dailyTargets);
        }
    }
}
