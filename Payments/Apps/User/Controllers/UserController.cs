using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Payments.Apps.AppSystem.Controllers;
using Payments.Apps.Org.Models;
using Payments.Apps.User.Helpers;
using Payments.Apps.User.Interfaces;
using Payments.Apps.User.Models;
using Payments.DTOs;
using System.Data;
using System.Security.Claims;

namespace Payments.Apps.User.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login/email")]
        public async Task<IActionResult> LoginWithEmail([FromBody] LoginDto loginDto)
        {
            if (loginDto == null || string.IsNullOrEmpty(loginDto.Email) || string.IsNullOrEmpty(loginDto.Password))
            {
                return BadRequest("Email and password are required.");
            }

            var result = await _userService.LoginWithEmail(loginDto.Email, loginDto.Password);

            return result.Match<IActionResult>(
                user =>
                {
                    var roles = user.Roles ?? new List<string>();

                    var token = TokenHelper.GenerateJwtToken(loginDto.Email, roles);

                    var response = new
                    {
                        User = user,
                        Token = "Bearer " + token,
                        Roles = roles
                    };
                    return Ok(response);
                },
                errors =>
                {
                    var problemDetails = new ProblemDetails
                    {
                        Title = "Login failed",
                        Detail = "One or more errors occurred.",
                        Status = StatusCodes.Status400BadRequest,
                        Extensions = { { "errors", errors } }
                    };
                    return BadRequest(new { isError = true, problemDetails });
                }
            );
        }

        [HttpPost("register/email")]
        public async Task<IActionResult> RegisterWithEmail([FromBody] RegisterDto registerDto)
        {
            ErrorOr<dynamic> result = await _userService.RegisterWithEmail(registerDto);
            return result.Match<IActionResult>(
                user => Ok(user),
                errors => Problem(errors.ToString())
            );
        }

        [HttpPost("register/mobile")]
        public async Task<IActionResult> RegisterWithMobile([FromBody] RegisterDto registerDto)
        {
            ErrorOr<dynamic> result = await _userService.RegisterWithMobile(registerDto);

            return result.Match(
                user => Ok(user),
                errors => Problem(errors.ToString())
            );
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            ErrorOr<bool> result = await _userService.ForgotPassword(forgotPasswordDto.EmailOrPhone);
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            ErrorOr<bool> result = await _userService.ResetPassword(resetPasswordDto.Token, resetPasswordDto.NewPassword);
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string token)
        {
            ErrorOr<bool> result = await _userService.VerifyEmail(token);
            return result.Match<IActionResult>(
                success => Ok("Email verified successfully."),
                errors => Problem(errors.ToString())
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (updateUserDto == null)
            {
                return BadRequest("Updated user data cannot be null.");
            }

            ErrorOr<UserModel> result = await _userService.UpdateUser(id, updateUserDto);
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [Authorize("Admin")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            ErrorOr<List<UserModel>> result = await _userService.GetAllUsers();
            return result.Match<IActionResult>(
                success => Ok(success),
                errors => Problem(errors.ToString())
            );
        }

        [Authorize("Admin")]
        [HttpGet("me")]
        public IActionResult GetAuthenticatedUser()
        {
            var user = HttpContext.User;

            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return Unauthorized(new { Message = "No user is logged in." });
            }

            var username = user.Identity.Name;
            var roles = user.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                Username = username,
                Roles = roles
            });
        }

    }

}
