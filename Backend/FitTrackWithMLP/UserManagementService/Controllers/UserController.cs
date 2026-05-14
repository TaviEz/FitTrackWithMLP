using FitTrackWithMLP.Shared.DTOs;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementService.Context;
using UserManagementService.Models;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public UserController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return Unauthorized();

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email!)
                ]),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            return Ok(new { accessToken = token });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetLoggedUser()
        {
            var userId = _userManager.GetUserId(User);
            return Ok(userId);
        }


        [Authorize]
        [HttpPost("details")]
        public async Task<IActionResult> StoreUserDetails([FromBody] UserDetailsDto userDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(ud => ud.UserDetails)
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");

            if (!Enum.TryParse(userDto.Gender, true, out Gender userGender))
                return BadRequest("Invalid gender value");

            if (!Enum.TryParse(userDto.ActivityLevel, true, out ActivityLevel userActivityLevel))
                return BadRequest("Invalid activity level");

            if (!Enum.TryParse(userDto.Goal, true, out GoalType userGoalType))
                return BadRequest("Invalid goal type");

            if (user.UserDetails == null)
            {
                var newUserDetails = new UserDetails
                {
                    UserId = userId,
                    Gender = userGender,
                    Age = userDto.Age,
                    Weight = userDto.Weight,
                    Height = userDto.Height,
                    ActivityLevel = userActivityLevel,
                    Date = DateTime.Now,
                    Bmr = userDto.Bmr,
                    Tdee = userDto.Tdee,
                    GoalType = userGoalType
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
                user.UserDetails.Bmr = userDto.Bmr;
                user.UserDetails.Tdee = userDto.Tdee;
                user.UserDetails.GoalType = userGoalType;

                var result = await _dbContext.SaveChangesAsync();   
                if (result > 0)
                    return Ok();
                else
                    return StatusCode(500, "Failed to save user details.");
            }

            return StatusCode(500, $"User details for user {userId} already created");
        }
    }
}
