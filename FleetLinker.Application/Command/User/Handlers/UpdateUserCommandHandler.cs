using FleetLinker.Application.Common.Caching;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace FleetLinker.Application.Command.User.Handlers
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserAsyncCommand, APIResponse<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAppLocalizer _localizer;
        private readonly ICacheService _cache;

        public UpdateUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IAppLocalizer localizer,
            ICacheService cache)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _localizer = localizer;
            _cache = cache;
        }

        public async Task<APIResponse<bool>> Handle(UpdateUserAsyncCommand request, CancellationToken cancellationToken)
        {
            var dto = request.UpdateUserDto;
            if (dto == null)
                return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, null, _localizer[LocalizationMessages.UserUpdateDataRequired]);

            var user = await _userManager.FindByIdAsync(dto.Id);
            if (user == null)
                return APIResponse<bool>.Fail(StatusCodes.Status404NotFound, null, _localizer[LocalizationMessages.UserNotFound]);

            // Validate Email/Mobile uniqueness if changed
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                if (await _userManager.Users.AnyAsync(u => u.Id != dto.Id && u.Email == dto.Email && !u.IsDeleted, cancellationToken))
                    return APIResponse<bool>.Fail(StatusCodes.Status409Conflict, null, _localizer[LocalizationMessages.EmailAlreadyInUse]);
            }

            if (!string.IsNullOrWhiteSpace(dto.Mobile) && dto.Mobile != user.PhoneNumber)
            {
                if (await _userManager.Users.AnyAsync(u => u.Id != dto.Id && u.PhoneNumber == dto.Mobile && !u.IsDeleted, cancellationToken))
                    return APIResponse<bool>.Fail(StatusCodes.Status409Conflict, null, _localizer[LocalizationMessages.MobileAlreadyInUse]);
            }

            user.FullName = dto.FullName ?? user.FullName;
            user.Email = dto.Email ?? user.Email;
            user.PhoneNumber = dto.Mobile ?? user.PhoneNumber;
            user.UserName = dto.Username ?? user.UserName;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = updateResult.Errors.Select(e => e.Description).ToList();
                return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, errors, string.Join("; ", errors));
            }

            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.Password);
                if (!passwordResult.Succeeded)
                {
                    var errors = passwordResult.Errors.Select(e => e.Description).ToList();
                    return APIResponse<bool>.Fail(StatusCodes.Status400BadRequest, errors, string.Join("; ", errors));
                }
            }

            if (dto.RoleIds?.Any() == true)
            {
                var oldRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, oldRoles);

                var roleNames = await _roleManager.Roles
                    .Where(r => dto.RoleIds.Contains(r.Id))
                    .Select(r => r.Name!)
                    .ToListAsync(cancellationToken);

                await _userManager.AddToRolesAsync(user, roleNames);
            }

            await _cache.RemoveAsync(CacheKeys.UsersAll, cancellationToken);
            await _cache.RemoveAsync(CacheKeys.UserById(dto.Id), cancellationToken);

            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.UserUpdatedSuccessfully]);
        }
    }
}
