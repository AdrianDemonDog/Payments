using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Payments.Apps.User.Helpers
{
    public class TokenHelper
    {
        private static readonly string SecretKey = "k8J5G@3pZr#Yd!2NxLfE$9QvT*Wb^Rm&Cj7AoXhKsU6MqV1Pn"; // Replace with your actual secret key
        private static readonly byte[] Key = Encoding.ASCII.GetBytes(SecretKey);

        public static string GenerateToken(string emailOrPhone)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, emailOrPhone) }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static bool ValidateToken(string token, out string? emailOrPhone)
        {
            emailOrPhone = null;
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                emailOrPhone = principal.Identity?.Name;
                return emailOrPhone != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
