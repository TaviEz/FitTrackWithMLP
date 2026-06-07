using FitTrackWithMLP.Shared.DTOs.Authentication;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UserManagementService.Models;
using UserManagementService.Services.Authentication;
using UserManagementService.Services.UserDetails;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IIdentityService _authService;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly ILogger<UserController> _logger;
        private readonly IWebHostEnvironment _env;

        public UserController(
            IIdentityService authService,
            ITokenService tokenService,
            IRefreshTokenService refreshTokenService,
            IUserDetailsService userDetailsService,
            ILogger<UserController> logger,
            IWebHostEnvironment env)
        {
            _authService = authService;
            _tokenService = tokenService;
            _refreshTokenService = refreshTokenService;
            _userDetailsService = userDetailsService;
            _logger = logger;
            _env = env;
        }


        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (user is null)
            {
                _logger.LogWarning("Login failed for email: {Email}", loginDto.Email);
                return Unauthorized();
            }

            var token = _tokenService.CreateAccessToken(user);
            var refreshToken = await _refreshTokenService.GenerateAndStoreAsync(user.Id);

            bool isDev = _env.IsDevelopment();
            Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev,
                SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            _logger.LogInformation("Login successful for userId: {UserId}", user.Id);
            return Ok(new { accessToken = token });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var result = await _authService.RegisterAsync(registerDto.Email, registerDto.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Register successful for email: {Email}", registerDto.Email);
                return Ok();
            }

            var duplicateEmail = result.Errors.Any(e => e.Code == "DuplicateEmail");
            if (duplicateEmail)
            {
                _logger.LogWarning("Register conflict for email: {Email}", registerDto.Email);
                return Conflict("Email already in use");
            }

            _logger.LogWarning("Register failed for email: {Email}. Errors: {Errors}",
                registerDto.Email,
                string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}")));

            return BadRequest(result.Errors);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (refreshToken is null)
            {
                _logger.LogWarning("Refresh failed: missing refresh token cookie.");
                return Unauthorized();
            }

            var storedToken = await _refreshTokenService.ValidateAsync(refreshToken);
            if (storedToken is null)
            {
                _logger.LogWarning("Refresh failed: invalid or expired refresh token.");
                return Unauthorized();
            }

            var user = await _authService.FindByIdAsync(storedToken.UserId);
            if (user is null)
            {
                _logger.LogWarning("Refresh failed: user not found for token userId: {UserId}", storedToken.UserId);
                return Unauthorized();
            }

            await _refreshTokenService.DeleteAsync(refreshToken);
            var newRefreshToken = await _refreshTokenService.GenerateAndStoreAsync(user.Id);

            bool isDev = _env.IsDevelopment();
            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = !isDev,
                SameSite = isDev ? SameSiteMode.Lax : SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            var newAccessToken = _tokenService.CreateAccessToken(user);
            _logger.LogInformation("Refresh successful for userId: {UserId}", user.Id);

            return Ok(new { accessToken = newAccessToken });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var refreshToken = Request.Cookies["refresh_token"];
            if (refreshToken is not null)
            {
                await _refreshTokenService.DeleteAsync(refreshToken);
                Response.Cookies.Delete("refresh_token");
            }

            _logger.LogInformation("Logout completed for userId: {UserId}", userId);
            return Ok();
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetLoggedUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

            var userDetailsDto = await _userDetailsService.GetUserDetailsAsync(userId);

            if (userDetailsDto == null)
            {
                _logger.LogInformation("GetUserDetails found no details for userId: {UserId}", userId);
                return Ok(null);
            }

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

            var result = await _userDetailsService.UpsertUserDetailsAsync(userId, userDto);

            switch (result.Status)
            {
                case UserDetailsOperationStatus.Success:
                    _logger.LogInformation("User details stored for userId: {UserId}", userId);
                    return Ok();
                case UserDetailsOperationStatus.UserNotFound:
                    _logger.LogWarning("StoreUserDetails failed: user not found for userId: {UserId}", userId);
                    return NotFound();
                default:
                    _logger.LogError("StoreUserDetails failed to save for userId: {UserId}", userId);
                    return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
