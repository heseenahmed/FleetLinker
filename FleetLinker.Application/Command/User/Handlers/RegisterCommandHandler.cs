using FleetLinker.Application.Common;
using FleetLinker.Application.Common.Caching;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.Common.Interfaces;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using FleetLinker.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FleetLinker.Application.Command.User.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, APIResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAppLocalizer _localizer;
        private readonly ICacheService _cache;
        private readonly IUnitOfWork _uow;

        public RegisterCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IAppLocalizer localizer,
            ICacheService cache,
            IUnitOfWork uow)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _localizer = localizer;
            _cache = cache;
            _uow = uow;
        }

        public async Task<APIResponse<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var dto = request.userDto;
            if (dto == null)
                return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, null, _localizer[LocalizationMessages.RegistrationPayloadRequired]);

            // Business Validation
            var normalizedEmail = dto.Email.Trim().ToUpperInvariant();
            var emailExists = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted, cancellationToken);
            if (emailExists)
                return APIResponse<bool>.Fail(StatusCodes.Status409Conflict, null, _localizer[LocalizationMessages.EmailAlreadyInUse]);

            var mobileExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == dto.Mobile.Trim() && !u.IsDeleted, cancellationToken);
            if (mobileExists)
                return APIResponse<bool>.Fail(StatusCodes.Status409Conflict, null, _localizer[LocalizationMessages.MobileAlreadyInUse]);

            var user = new ApplicationUser
            {
                UserName = string.IsNullOrWhiteSpace(dto.Username)
                    ? dto.FullName.Replace(" ", "").Trim()
                    : dto.Username.Trim(),
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                NormalizedEmail = normalizedEmail,
                PhoneNumber = dto.Mobile.Trim(),
                IsActive = true,
                IsDeleted = false,
                RefreshToken = TokenGenerator.GenerateRefreshToken(),
                RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7),
            };

            await _uow.BeginTransactionAsync();
            try
            {
                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    await _uow.RollbackAsync();
                    var errors = createResult.Errors.Select(e => e.Description).ToList();
                    var message = string.Join("; ", errors);
                    return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, errors, message);
                }

                if (dto.RoleIds?.Any() == true)
                {
                    foreach (var roleId in dto.RoleIds)
                    {
                        var role = await _roleManager.FindByIdAsync(roleId.ToString());
                        if (role == null)
                        {
                            await _uow.RollbackAsync();
                            return APIResponse<bool>.Fail(StatusCodes.Status404NotFound, null, _localizer[LocalizationMessages.RoleNotFound]);
                        }

                        var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name!);
                        if (!addRoleResult.Succeeded)
                        {
                            await _uow.RollbackAsync();
                            var errors = addRoleResult.Errors.Select(e => e.Description).ToList();
                            var message = string.Join("; ", errors);
                            return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, errors, message);
                        }
                    }
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var password = !string.IsNullOrWhiteSpace(dto.Password) ? dto.Password : dto.Mobile;
                var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, password);

                if (!passwordResult.Succeeded)
                {
                    await _uow.RollbackAsync();
                    var errors = passwordResult.Errors.Select(e => e.Description).ToList();
                    var message = string.Join("; ", errors);
                    return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, errors, message);
                }

                await _uow.CommitAsync();
                await _cache.RemoveAsync(CacheKeys.UsersAll, cancellationToken);
                return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.UserRegisteredSuccessfully]);
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return APIResponse<bool>.Exception(ex);
            }
        }
    }
}
