using FitTrack.Context;
using FitTrack.DTOs;
using FitTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        [HttpGet("me")]
        public ActionResult GetLoggedUser()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return Unauthorized("User not authenticated");

            var userId = _userManager.GetUserId(User);
            return Ok(userId);
        }


        [HttpPost("details")]
        public async Task<ActionResult> StoreUserDetails([FromBody] UserDetailsDto userDto)
        {
            var user = await _dbContext.Users
                .Where(u => u.Id == userDto.Id)
                .Include(ud => ud.UserDetails)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            if (!Enum.TryParse(userDto.Gender, true, out Gender userGender))
                return BadRequest("Invalid gender value");

            if (!Enum.TryParse(userDto.ActivityLevel, true, out ActivityLevel userActivityLevel))
                return BadRequest("Invalid activity level");

            if(user.UserDetails == null)
            {
                var newUserDetails = new UserDetails
                {
                    Gender = userGender,
                    Age = userDto.Age,
                    Weight = userDto.Weight,
                    Height = userDto.Height,
                    ActivityLevel = userActivityLevel,
                    Date = DateTime.Now,
                    UserId = userDto.Id
                };

                _dbContext.UserDetails.Add(newUserDetails);
                var result = _dbContext.SaveChanges();

                if (result > 0)
                    return Ok();
                else
                    return StatusCode(500, "Failed to save user details.");
            }
            else // TODO: remove else after testing
            {
                user.UserDetails.Gender = userGender;
                user.UserDetails.Age = userDto.Age;
                user.UserDetails.Weight = userDto.Weight;
                user.UserDetails.Height = userDto.Height;
                user.UserDetails.ActivityLevel = userActivityLevel;
                user.UserDetails.Date = DateTime.Now;

                var result = await _dbContext.SaveChangesAsync();   
                if (result > 0)
                    return Ok();
                else
                    return StatusCode(500, "Failed to save user details.");
            }

            return StatusCode(500, $"User details for user {userDto.Id} already created");
        }
    }
}
