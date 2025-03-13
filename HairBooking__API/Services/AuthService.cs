using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HairBooking__API.Services
{
    public class AuthService(IConfiguration configuration)
    {
        private readonly string _secretKey = configuration["Jwt:Secret"] ?? throw new ArgumentNullException(nameof(configuration));
        private readonly string _issuer = configuration["Jwt:Issuer"] ?? throw new ArgumentNullException(nameof(configuration));
        private readonly string _audience = configuration["Jwt:Audience"] ?? throw new ArgumentNullException(nameof(configuration));

        public string GenerateJwtToken(string userId, string email, string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId),
                new(JwtRegisteredClaimNames.Email, email),
                new("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
