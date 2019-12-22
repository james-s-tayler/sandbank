using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Core.Jwt
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JwtTokenConfiguration _jwtTokenConfiguration;

        public JwtTokenService(IOptions<JwtTokenConfiguration> jwtTokenConfiguration) => _jwtTokenConfiguration = jwtTokenConfiguration.Value;
        
        public string GenerateToken(int userId)
        {
            var claims = new[] 
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtTokenConfiguration.Secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _jwtTokenConfiguration.Issuer,
                _jwtTokenConfiguration.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtTokenConfiguration.AccessExpiration),
                signingCredentials: signingCredentials,
                claims: claims);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);

        }
    }
}