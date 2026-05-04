using FitTrackWithMLP.Shared.DTOs;
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
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? User.FindFirst("sub")?.Value;

            //var userId = _userManager.GetUserId(User);
            //var userDetails = _dbContext.UserDetails.Where(ud => ud.UserId == userId).FirstOrDefault();
            //if (userDetails == null)
            //    return NotFound("User details not found");

            //var activityGroup = NutritionCalculator.GetGroup(userDetails.ActivityLevel);
            //// TODO: store users goal in userDetails
            //var goalType = GoalType.MaintainFormTrained;
            //var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userDetails, activityGroup, goalType);

            return Ok(userId);
        }
    }
}
