using AutoMapper;
using FitTrackWithMLP.Shared.DTOs.Authentication;
using FitTrackWithMLP.Shared.DTOs.User;
using FitTrackWithMLP.Shared.Enums;
using FitTrackWithMLP.Shared.Logic;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementService.Context;
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
        private readonly IAuthCookieService _authCookieService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IIdentityService authService,
            ITokenService tokenService,
            IAuthCookieService authCookieService,
            IUserDetailsService userDetailsService,
            ILogger<UserController> logger)
        {
            _authService = authService;
            _tokenService = tokenService;
            _authCookieService = authCookieService;
            _userDetailsService = userDetailsService;
            _logger = logger;
        }

        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _authService.AuthenticateAsync(loginDto.Email, loginDto.Password);
            if (user is null)
            {
                return Unauthorized();
            }

            var token = _tokenService.CreateAccessToken(user);
            _authCookieService.AppendAuthCookies(HttpContext, token);

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

            // TODO: handle register error in frontend
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

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _authCookieService.DeleteAuthCookies(HttpContext);

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

            // TODO: supress errors after login if userDetails is empty
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
            return result.Status switch
            {
                UserDetailsOperationStatus.Success => Ok(),
                UserDetailsOperationStatus.UserNotFound => NotFound(),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
    }
}
