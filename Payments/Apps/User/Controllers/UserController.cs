using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payments.Apps.AppSystem.Controllers;
using Payments.Apps.Org.Models;
using Payments.Apps.User.Helpers;
using Payments.Apps.User.Interfaces;
using Payments.Apps.User.Models;
using Payments.DTOs;

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
            ErrorOr<dynamic> result = await _userService.LoginWithEmail(loginDto.Email, loginDto.Password);
            return result.Match<IActionResult>(
                user => Ok(user),
                errors => Problem(errors.ToString())
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

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto verifyEmailDto)
        {
            ErrorOr<bool> result = await _userService.VerifyEmail(verifyEmailDto.Token);
            return result.Match<IActionResult>(
                success => Ok(success),
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

    }
}
