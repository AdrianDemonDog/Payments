using Microsoft.IdentityModel.Tokens;
using Payments.Apps.AppSystem.Helpers;
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

        public static string GenerateJwtToken(string username, IEnumerable<string> roles)
        {
            var secretKey = UtilityHelper.GetAppSetting("Jwt:SecretKey");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Crear claims base
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, username)
            };

            // Agregar roles como claims
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = new JwtSecurityToken(
                issuer: "PaymentsAPI",
                audience: "PaymentsAPP",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}