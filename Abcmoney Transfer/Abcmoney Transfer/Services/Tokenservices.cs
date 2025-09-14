using System.Security.Claims;
using System.Text;

namespace Abcmoney_Transfer.Services
{
    public class TokenService(IConfiguration configuration)
    {
        private readonly string _secretKey = configuration["JwtSettings:SecretKey"];
        private readonly string _refreshSecretKey = configuration["JwtSettings:RefreshSecretKey"];

        public (string AccessToken, string RefreshToken) GenerateTokens(AppUser user, IList<string> roles)
        {
            var accessToken = GenerateAccessToken(user, roles);
            var refreshToken = GenerateRefreshToken(user, roles);

            return (AccessToken: accessToken, RefreshToken: refreshToken);
        }

        private string GenerateAccessToken(AppUser user, IList<string> roles)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Define claims with all required data
            var claims = new List<Claim>
            {
             new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
             new Claim(JwtRegisteredClaimNames.Name, user.UserName),
             new Claim(JwtRegisteredClaimNames.Email, user.Email),
             new Claim("firstName", user.FirstName),
             new Claim("middleName", user.MiddleName),
             new Claim("LastName", user.LastName),
             new Claim("role", roles.FirstOrDefault() ?? "User"),
             new Claim("RoleIds", string.Join(",", roles)),
             new Claim("IdUid", user.Id.ToString()),
             new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
             new Claim(JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddDays(5).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim("sc", "web")
            };

            /*  // Add additional roles as separate claims
              foreach (var role in roles)
              {
                  claims.Add(new Claim(ClaimTypes.Role, role));
              }*/


            var token = new JwtSecurityToken(
                issuer: "ABSExchange",
                audience: "ABSExchange",
                claims: claims,
                expires: DateTime.Now.AddDays(5), // Access token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken(AppUser user, IList<string> roles)
        {
            var refreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshSecretKey));
            var credentials = new SigningCredentials(refreshKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var token = new JwtSecurityToken(
                issuer: "ABSExchange",
                audience: "ABSExchange",
                claims: claims,
                expires: DateTime.Now.AddDays(7), // Refresh token expiration time
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
