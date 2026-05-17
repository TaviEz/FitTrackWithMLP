using AutoMapper;
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

            if (user.UserDetails == null)
            {
                var newUserDetails = _mapper.Map<UserDetails>(userDto);
                newUserDetails.UserId = userId;
                newUserDetails.Date = DateTime.UtcNow;

                _dbContext.UserDetails.Add(newUserDetails);
                var result = _dbContext.SaveChanges();

                if (result > 0)
                    return Ok();
                else
                    return StatusCode(500, "Failed to save user details.");
            }
            else // TODO: remove else after testing or create a separate endpoint for updating user details
            {
                _mapper.Map(userDto, user.UserDetails);
                user.UserDetails.Date = DateTime.UtcNow;

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
