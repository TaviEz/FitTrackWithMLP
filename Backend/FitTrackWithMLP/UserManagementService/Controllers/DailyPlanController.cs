using UserManagementService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagementService.Context;
using UserManagementService.Logic;
using static UserManagementService.Logic.NutritionCalculator;

namespace UserManagementService.Controllers
{
    // TODO: add authorization for the API's
    [Route("api/[controller]")]
    [ApiController]
    public class DailyPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public DailyPlanController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("generate")]
        public IActionResult GenerateDailyPlan()
        {
            var userId = _userManager.GetUserId(User);            
            var userDetails = _dbContext.UserDetails.Where(ud => ud.UserId == userId).FirstOrDefault();
            if (userDetails == null)
                return NotFound("User details not found");

            var activityGroup = NutritionCalculator.GetGroup(userDetails.ActivityLevel);
            // TODO: store users goal in userDetails
            var goalType = GoalType.MaintainFormTrained;
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(userDetails, activityGroup, goalType);

            return Ok();
        }
    }
}
