using ErrorOr;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Payments.Apps.AppSystem.Controllers;
using Payments.Apps.Org.Models;
using DemonDog.Contracts.Models;
using Payments.Apps.User.Helpers;
using Payments.Apps.User.Interfaces;
using Payments.Apps.User.Models;
using Payments.Common.Events;
using Payments.DTOs;
using RabbitMQ.Client;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Payments.Apps.User.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IBus _bus;

        public UserController(IUserService userService, IConfiguration configuration, IBus bus, HttpClient httpClient)
        {
            _userService = userService;
            _configuration = configuration;
            _bus = bus;
            _httpClient = httpClient;
        }

        [HttpPost("fake-login")]
        public async Task<IActionResult> FakeLogin()
        {
            var fakeUser = new
            {
                Id = Guid.NewGuid(),
                Name = "Takeshi Nakamura",
                Email = "takeshi.nakamura@example.com",
                Roles = new List<string> { "User", "Admin" }
            };

            var token = GenerateJwt(fakeUser);

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("http://localhost:5000/identity/Auth/all-users");

            var users = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                Message = "Fake login successful",
                Token = token,
                Users = users
            });
        }

        private string GenerateJwt(dynamic user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("k8J5G@3pZr#Yd!2NxLfE$9QvT*Wb^Rm&Cj7AoXhKsU6MqV1Pn"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            claims.AddRange(((IEnumerable<string>)user.Roles).Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: "fake-issuer",
                audience: "fake-audience",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
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
                        Roles = roles,
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

        [HttpPut("update-user/{id}")]
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

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                    UserName = "guest",
                    Password = "guest"
                };

                using var connection = await factory.CreateConnectionAsync();
                using var channel = await connection.CreateChannelAsync();

                return Ok("✅ Conexión exitosa a RabbitMQ");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"❌ Error conectando a RabbitMQ: {ex.Message}");
            }
        }

        [HttpGet("profile")]
        public IActionResult GetUserProfile([FromQuery] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = UserHelper.GetUserByEmail(email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        [HttpPost("decode-token")]
        public IActionResult DecodeToken([FromBody] TokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("Token is required.");
            }

            try
            {
                var token = request.Token.StartsWith("Bearer ") ? request.Token.Substring(7) : request.Token;

                var secretKey = _configuration["Jwt:SecretKey"];
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var handler = new JwtSecurityTokenHandler();

                var validations = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key
                };

                var claimsPrincipal = handler.ValidateToken(token, validations, out SecurityToken validatedToken);
                var jwtToken = validatedToken as JwtSecurityToken;

                if (jwtToken == null)
                {
                    return BadRequest("Invalid token.");
                }

                // Agrupar claims con la misma clave en listas
                var claims = jwtToken.Claims
                    .GroupBy(c => c.Type)
                    .ToDictionary(g => g.Key, g => g.Select(c => c.Value).ToList());

                return Ok(new
                {
                    Valid = true,
                    Claims = claims
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Valid = false, Error = ex.Message });
            }
        }

    }

    // Modelo para la solicitud
    public class TokenRequest
    {
        public string Token { get; set; }
    }
}
