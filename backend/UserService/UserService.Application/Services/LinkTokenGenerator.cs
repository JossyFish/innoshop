using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.Application.Interfaces;
using UserService.Domain.Models;

namespace UserService.Application.Services
{
    public class LinkTokenGenerator : ILinkTokenGenerator
    {
        private readonly LinkTokenOptions _options;
        ILogger<LinkTokenGenerator> _logger;

        public LinkTokenGenerator (IOptions<LinkTokenOptions> options, ILogger<LinkTokenGenerator> logger)
        {
            _options = options.Value;
            _logger = logger;
        }

        public string GenerateToken(string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, email)
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: signingCredentials,
                expires: DateTime.UtcNow.AddMinutes(_options.ExpiresMinutes));

            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenValue;
        }
        public string? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_options.SecretKey);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                var email = principal.FindFirst(ClaimTypes.Email)?.Value;

                return email; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate token: {token}", token);
                return null;
            }
        }
    }
}
