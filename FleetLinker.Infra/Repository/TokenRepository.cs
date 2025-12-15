using Benzeny.Domain.Entity;
using Benzeny.Domain.IRepository;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Benzeny.Infra.Repository
{
    public class TokenRepository : ITokenRepository
    {
        private readonly JwtSettings _jwtSettings;

        public TokenRepository(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        public Task<ClaimsPrincipal> GetPrincipalFromExpiredToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token is required.", nameof(token));

            var validationParams = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidAudience = _jwtSettings.Audience,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false,          // allow expired tokens (we're just extracting principal)
                ClockSkew = TimeSpan.Zero
            };

            var handler = new JwtSecurityTokenHandler();

            try
            {
                var principal = handler.ValidateToken(token, validationParams, out var securityToken);

                if (securityToken is not JwtSecurityToken jwt ||
                    !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new UnauthorizedAccessException("Invalid token algorithm.");
                }

                return Task.FromResult(principal);
            }
            catch (SecurityTokenException ex)
            {
                // Invalid signature, malformed, wrong issuer/audience, etc.
                throw new UnauthorizedAccessException("Invalid access token.", ex);
            }
            catch (Exception ex)
            {
                // Any other parsing issue → unauthorized for consistency with auth flow
                throw new UnauthorizedAccessException("Invalid or malformed token.", ex);
            }
        }
    }
}
