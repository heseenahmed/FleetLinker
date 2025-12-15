using Amazon.Runtime.Internal.Util;
using Benzeny.API.Controllers;
using Benzeny.Application.Command.User;
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using Benzeny.Domain.Entity.Dto.Identity;
using BenzenyMain.Application.Common;
using BenzenyMain.Domain.Entity.Dto.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class AuthController : ApiController
{
    private readonly JwtSettings _jwtSettings;
    private readonly IDistributedCache _cache;
    public AuthController(
        ISender mediator,
        UserManager<ApplicationUser> userManager,
        JwtSettings jwtSettings,
        IDistributedCache cache) : base(mediator, userManager)
    {
        _jwtSettings = jwtSettings;
        _cache = cache;
    }

    // POST: api/Auth/login
    // Keeps existing behavior because your LoginCommand currently returns an APIResponse with ApiStatusCode.
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new LoginCommand(request), ct);
        return StatusCode(result.ApiStatusCode, result);
    }

    // POST: api/Auth/logout
    // logoutAll = true يقطع كل الأجهزة (يحدّث SecurityStamp)
    [HttpPost("logout")]
    [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Logout()
    {
        bool logoutAll = true;
        var user = await _userManager.GetUserAsync(User)
                   ?? throw new UnauthorizedAccessException("User not authenticated.");

        // 1) امسح الـ Refresh Token للجهاز الحالي (أو احفظ لكل جلسة إن وجد)
        user.RefreshToken = null;
        user.RefreshTokenExpiryUTC = null;

        if (logoutAll)
        {
            // 2A) Logout كل الأجهزة: أبطل كل التوكنات بتحديث الـ SecurityStamp
            await _userManager.UpdateSecurityStampAsync(user);
        }
        else
        {
            // 2B) Logout الجهاز الحالي: بلّك ليست الـ Access Token الحالي حتى انتهاء صلاحيته
            var jti = User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var expStr = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;

            if (!string.IsNullOrWhiteSpace(jti) && long.TryParse(expStr, out var expUnix))
            {
                var exp = DateTimeOffset.FromUnixTimeSeconds(expUnix);
                var ttl = exp - DateTimeOffset.UtcNow;
                if (ttl < TimeSpan.Zero) ttl = TimeSpan.FromSeconds(5); // أمان

                //// مفتاح البلوك ليست
                var key = $"blk:{jti}";
                await _cache.SetStringAsync(
                    key,
                    "1",
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = ttl
                    });

            }
        }

        var update = await _userManager.UpdateAsync(user);
        if (!update.Succeeded)
            throw new InvalidOperationException(string.Join("; ", update.Errors.Select(e => e.Description)));

        return Ok(APIResponse<object>.Success(null, logoutAll
            ? "Logout successful (all devices)."
            : "Logout successful."));
    }

    // POST: api/Auth/change-password
    [HttpPost("change-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordApi change, CancellationToken ct)
    {
        if (change == null ||
            string.IsNullOrWhiteSpace(change.OldPassword) ||
            string.IsNullOrWhiteSpace(change.NewPassword))
        {
            throw new ArgumentException("Old and new passwords are required.");
        }

        //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        //             ?? throw new UnauthorizedAccessException("User is not authorized.");

        var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == change.UserId, ct)
                   ?? throw new KeyNotFoundException("User not found.");
        
        var roles = await _userManager.GetRolesAsync(user);

        var accessToken = TokenGenerator.GenerateAccessToken(user, roles, _jwtSettings);
        ChangePasswordResponseDto response = new ChangePasswordResponseDto();
        response.AccessToken = accessToken;
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, change.OldPassword);

        if (!isPasswordValid)
            throw new UnauthorizedAccessException("Invalid credentials.");

        var result = await _userManager.ChangePasswordAsync(user, change.OldPassword, change.NewPassword);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        user.FirstTimeLogin = false;
        await _userManager.UpdateAsync(user);
        return Ok(APIResponse<object>.Success(response, "Password changed successfully."));
    }

    // POST: api/Auth/AddNewPassword
    [HttpPost("AddNewPassword")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddNewPassword([FromBody] ForgetPasswordApi forget, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(forget?.Mobile) || string.IsNullOrWhiteSpace(forget.Password))
            throw new ArgumentException("Mobile and password are required.");

        var user = await _userManager.Users
            .FirstOrDefaultAsync(e => e.PhoneNumber == forget.Mobile && e.Active, ct)
            ?? throw new ApplicationException("User not found.");

        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, forget.Password);
        await _userManager.UpdateAsync(user);
        await _userManager.UpdateSecurityStampAsync(user);

        return Ok(APIResponse<object>.Success(null, "Password changed successfully."));
    }

    // POST: api/Auth/refresh
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(APIResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(APIResponse<string>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest request, CancellationToken ct)
    {
        // Validate expired access token & extract identity
        var principal = await _mediator.Send(new GetPrincipalFromExpiredTokenCommand(request.AccessToken), ct)
                        ?? throw new UnauthorizedAccessException("Invalid access token.");

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? throw new UnauthorizedAccessException("Invalid access token.");

        var user = await _userManager.FindByIdAsync(userId)
                   ?? throw new KeyNotFoundException("User not found.");

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryUTC <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        // Issue new tokens
        var newAccessToken = TokenGenerator.GenerateAccessToken(user, await _userManager.GetRolesAsync(user), _jwtSettings);

        // Rotate refresh token
        user.RefreshToken = TokenGenerator.GenerateRefreshToken();
        user.RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Ok(APIResponse<object>.Success(new
        {
            AccessToken = newAccessToken,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiry = user.RefreshTokenExpiryUTC
        }, "Token refreshed successfully."));
    }
}
