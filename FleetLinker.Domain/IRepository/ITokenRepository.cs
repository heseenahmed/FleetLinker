
using System.Security.Claims;

namespace FleetLinker.Domain.IRepository
{
    public interface ITokenRepository
    {
        //string GenerateAccessToken(IEnumerable<Claim> claims, int expiryMinutes = 0);
        //string GenerateRefreshToken();
        Task<ClaimsPrincipal>? GetPrincipalFromExpiredToken(string? token);
    }
}
