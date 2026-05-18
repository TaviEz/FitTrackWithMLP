using AutoMapper;
using FitTrackWithMLP.Shared.DTOs;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums;
using FitTrackWithMLP.Shared.Logic;
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
        private readonly IMapper _mapper;

        public UserController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
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

        // TODO: finish this api
        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var userDetails = await _dbContext.UserDetails
                .Where(ud => ud.UserId == userId)
                .FirstOrDefaultAsync();

            var userDetailsDto = _mapper.Map<UserDetailsDto>(userDetails);  

            return Ok(userDetailsDto);
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

            // get the daily targets for the user
            var activityGroup = NutritionCalculator.GetGroup(userDto.ActivityLevel);
            var dailyTargets = NutritionCalculator.GetDailyTargetsForGoal(
                userDto.Weight, userDto.Tdee, activityGroup, userDto.Goal
            );

            if (user.UserDetails == null)
            {
                var newUserDetails = _mapper.Map<UserDetails>(userDto);
                newUserDetails.UserId = userId;
                newUserDetails.Date = DateTime.UtcNow;
                newUserDetails.TargetCalories = dailyTargets.Calories;

                _dbContext.UserDetails.Add(newUserDetails);
                var result = _dbContext.SaveChanges();

                if (result > 0)
                    return Ok();
                else
                    return StatusCode(500, "Failed to save user details.");
            }
            else
            {
                _mapper.Map(userDto, user.UserDetails);
                user.UserDetails.Date = DateTime.UtcNow;
                user.UserDetails.TargetCalories = dailyTargets.Calories;

                var result = await _dbContext.SaveChangesAsync();   
                if (result > 0)
                    return Ok();
                else
                    return StatusCode(500, "Failed to save user details.");
            }
        }
    }
}
