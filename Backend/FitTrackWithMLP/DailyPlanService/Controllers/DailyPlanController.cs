using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DailyPlanService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DailyPlanController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult GenerateDailyPlan()
        {
            //var userId = _userManager.GetUserId(User);
            //var userDetails = _dbContext.UserDetails.Where(ud => ud.UserId == userId).FirstOrDefault();
            //if (userDetails == null)
            //    return NotFound("User details not found");

            //var activityGroup = NutritionCalculator.GetGroup(userDetails.ActivityLevel);
            //// TODO: store users goal in userDetails
            //var goalType = GoalType.MaintainFormTrained;
            //var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userDetails, activityGroup, goalType);

            return Ok();
        }
    }
}
