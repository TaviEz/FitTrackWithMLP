using AutoMapper;
using FitTrackWithMLP.Shared.DTOs.Authentication;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Logic;
using Microsoft.AspNetCore.Antiforgery;
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
        private readonly IAntiforgery _antiforgery;
        private readonly ILogger<UserController> _logger;

        public UserController(
            ApplicationDbContext dbContext,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IMapper mapper,
            ILogger<UserController> logger,
            IAntiforgery antiforgery)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _configuration = configuration;
            _mapper = mapper;
            _logger = logger;
            _antiforgery = antiforgery;
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                return Unauthorized();
            }

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

            // add the token in the cookie
            Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // use HTTPS
                SameSite = SameSiteMode.None, // needed for frontend on different origin
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            // add the XSRF token in the cookie
            var xsrfTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            Response.Cookies.Append("XSRF-TOKEN", xsrfTokens.RequestToken!, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            _logger.LogInformation("Login successful for userId: {UserId}", user.Id);
            return Ok(new { accessToken = token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Register conflict for email: {Email}", registerDto.Email);
                return Conflict("Email already in use");
            }

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Register failed for email: {Email}. Errors: {Errors}",
                        registerDto.Email,
                        string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));
                return BadRequest(result.Errors);
            }

            _logger.LogInformation("Register successful for userId: {UserId}", user.Id);
            return Ok();
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Response.Cookies.Delete("access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            _logger.LogInformation("Logout completed for userId: {UserId}", userId);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetLoggedUser()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GetLoggedUser could not resolve user id.");
                return Unauthorized();
            }

            _logger.LogInformation("GetLoggedUser resolved userId: {UserId}", userId);
            return Ok(userId);
        }

        [Authorize]
        [HttpGet("details")]
        public async Task<IActionResult> GetUserDetails()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GetUserDetails unauthorized: user id not found.");
                return Unauthorized();
            }

            var userDetails = await _dbContext.UserDetails
                .Where(ud => ud.UserId == userId)
                .FirstOrDefaultAsync();

            if (userDetails == null)
            {
                _logger.LogInformation("GetUserDetails found no details for userId: {UserId}", userId);
                return Ok(null);
            }

            var userDetailsDto = _mapper.Map<UserDetailsDto>(userDetails);
            _logger.LogInformation("GetUserDetails success for userId: {UserId}", userId);

            return Ok(userDetailsDto);
        }


        [Authorize]
        [HttpPost("details")]
        public async Task<IActionResult> StoreUserDetails([FromBody] UserDetailsDto userDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("StoreUserDetails unauthorized: user id not found.");
                return Unauthorized();
            }

            var user = await _dbContext.Users
                .Where(u => u.Id == userId)
                .Include(ud => ud.UserDetails)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("StoreUserDetails failed. User not found for userId: {UserId}", userId);
                return NotFound("User not found");
            }

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
                {
                    _logger.LogInformation("StoreUserDetails created details for userId: {UserId}", userId);
                    return Ok();
                }

                _logger.LogError("StoreUserDetails failed to create details for userId: {UserId}", userId);
                return StatusCode(500, "Failed to save user details.");
            }
            else
            {
                _mapper.Map(userDto, user.UserDetails);
                user.UserDetails.Date = DateTime.UtcNow;
                user.UserDetails.TargetCalories = dailyTargets.Calories;

                var result = await _dbContext.SaveChangesAsync();
                if (result > 0)
                {
                    _logger.LogInformation("StoreUserDetails updated details for userId: {UserId}", userId);
                    return Ok();
                }

                _logger.LogError("StoreUserDetails failed to update details for userId: {UserId}", userId);
                return StatusCode(500, "Failed to save user details.");
            }
        }
    }
}
