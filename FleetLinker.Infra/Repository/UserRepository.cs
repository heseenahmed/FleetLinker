using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto.Identity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;
using FleetLinker.Application.Common;
using FleetLinker.Domain.Entity.Dto.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;
using FleetLinker.Application.Common.Localization;
using FleetLinker.API.Resources;
namespace FleetLinker.Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IAppLocalizer _Localizer;
        public UserRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IAppLocalizer localizer
           )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _Localizer = localizer;
        }
        public async Task<UserInfoAPI?> GetUserInfoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(_Localizer[LocalizationMessages.UserIdRequired]);
            try
            {
                return await _context.Users
                    .Where(e => e.Id == id && e.IsActive && !e.IsDeleted)
                    .Select(s => new UserInfoAPI
                    {
                        FullName = s.FullName,
                        Mobile = s.PhoneNumber,
                        Id = s.Id
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(_Localizer[LocalizationMessages.ErrorRetrievingUserInfo], ex);
            }
        }
        public async Task<bool> UpdateUserAsync(UserForUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException( _Localizer[LocalizationMessages.UserUpdateDataRequired]);
            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new ArgumentException(_Localizer[LocalizationMessages.UserIdRequired]);
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(dto.Id);
                if (user == null)
                    throw new KeyNotFoundException(_Localizer[LocalizationMessages.UserNotFound]);
                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailExists = await _userManager.Users
                        .AnyAsync(u => u.Id != dto.Id &&
                                       u.Email == dto.Email &&
                                       !u.IsDeleted);
                    if (emailExists)
                        throw new ArgumentException(_Localizer[LocalizationMessages.EmailAlreadyInUse]);
                }
                if (!string.IsNullOrWhiteSpace(dto.Mobile))
                {
                    var phoneExists = await _userManager.Users
                        .AnyAsync(u => u.Id != dto.Id &&
                                       u.PhoneNumber == dto.Mobile &&
                                       !u.IsDeleted);
                    if (phoneExists)
                        throw new ArgumentException(_Localizer[LocalizationMessages.MobileAlreadyInUse]);
                }
                user.FullName = dto.FullName ?? user.FullName;
                user.Email = dto.Email ?? user.Email;
                user.PhoneNumber = dto.Mobile ?? user.PhoneNumber;
                user.UserName = dto.Username ?? user.UserName;
                user.EmailConfirmed = true;
                user.IsActive = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new ApplicationException(_Localizer[LocalizationMessages.FailedToUpdateUser]);
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.Password);
                    if (!passwordResult.Succeeded)
                        throw new ApplicationException(_Localizer[LocalizationMessages.FailedToResetPassword]);
                }
                var oldRoles = await _userManager.GetRolesAsync(user);
                if (oldRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, oldRoles);
                    if (!removeResult.Succeeded)
                        throw new ApplicationException(_Localizer[LocalizationMessages.ErrorDeleteRole]);
                }
                if (dto.RoleIds?.Any() == true)
                {
                    var roleNames = await _roleManager.Roles
                        .Where(r => dto.RoleIds.Contains(r.Id))
                        .Select(r => r.Name!)
                        .ToListAsync();
                    if (!roleNames.Any())
                        throw new ArgumentException(_Localizer[LocalizationMessages.RoleNotFound]);
                    var addResult = await _userManager.AddToRolesAsync(user, roleNames);
                    if (!addResult.Succeeded)
                        throw new ApplicationException(_Localizer[LocalizationMessages.FailedToAssignRoles]);
                }
                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> RegisterAsync(UserForRegisterDto request)
        {
            if (request == null)
                throw new ArgumentNullException(_Localizer[LocalizationMessages.RegistrationPayloadRequired]);
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException(_Localizer[LocalizationMessages.FullNameRequired]);
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException(_Localizer[LocalizationMessages.EmailRequired]);
            if (string.IsNullOrWhiteSpace(request.Mobile))
                throw new ArgumentException(_Localizer[LocalizationMessages.MobileRequired]);
            var normalizedEmail = request.Email.Trim().ToUpperInvariant();
            var existingUsers = await _context.Users
                .Where(u => u.NormalizedEmail == normalizedEmail && !u.IsDeleted)
                .ToListAsync();
            if (existingUsers.Count > 1)
                throw new InvalidOperationException(_Localizer[LocalizationMessages.DuplicateEmail]);
            
            if (existingUsers.Count == 1)
                throw new ArgumentException(_Localizer[LocalizationMessages.EmailAlreadyInUse]);
            var refreshToken = TokenGenerator.GenerateRefreshToken();
            var user = new ApplicationUser
            {
                UserName = string.IsNullOrWhiteSpace(request.Username)
                    ? request.FullName.Replace(" ", "").Trim()
                    : request.Username.Trim(),
                FullName = request.FullName.Trim(),
                Email = request.Email.Trim(),
                NormalizedEmail = normalizedEmail,
                PhoneNumber = request.Mobile.Trim(),
                IsActive = true,
                IsDeleted = false,
                RefreshToken = refreshToken,
                RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7),
            };
            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new ApplicationException(_Localizer[LocalizationMessages.FailedToCreateUser]);
            if (request.RoleIds?.Any() == true)
            {
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId.ToString());
                    if (role == null)
                        throw new KeyNotFoundException(_Localizer[LocalizationMessages.RoleNotFound]);
                    var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name!);
                    if (!addRoleResult.Succeeded)
                        throw new ApplicationException(_Localizer[LocalizationMessages.FailedToAssignRoles]);
                }
            }
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var password = !string.IsNullOrWhiteSpace(request.Password)
                ? request.Password
                : request.Mobile;
            var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
            if (!passwordResult.Succeeded)
                throw new ApplicationException(_Localizer[LocalizationMessages.FailedToResetPassword]);
            return true;
        }
        public async Task<bool> SwitchUserActiveAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(_Localizer[LocalizationMessages.UserIdRequired]);

            var user = await _userManager.FindByIdAsync(id)
                ?? throw new KeyNotFoundException(_Localizer[LocalizationMessages.UserNotFound]);

            user.IsActive = !user.IsActive;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApplicationException(_Localizer[LocalizationMessages.FailedToUpdateUser]);

            // ?? VERY IMPORTANT: invalidate all existing tokens
            await _userManager.UpdateSecurityStampAsync(user);

            return user.IsActive;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException(_Localizer[LocalizationMessages.UserIdRequired]);
            try
            {
                return await _context.Users
                    .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(_Localizer[LocalizationMessages.ErrorRetrievingUserInfo]);
            }
        }
        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            try
            {
                return await _context.Users
                    .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .Where(x => !x.IsDeleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(_Localizer[LocalizationMessages.ErrorRetrievingUserInfo]);
            }
        }
       
        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException(_Localizer[LocalizationMessages.UserIdRequired]);

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new KeyNotFoundException(_Localizer[LocalizationMessages.UserNotFound]);

            if (user.IsDeleted)
                throw new KeyNotFoundException(_Localizer[LocalizationMessages.UserAlreadyDeleted]);

            user.IsDeleted = true;
            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApplicationException(_Localizer[LocalizationMessages.FailedToDeleteUser]);

            // ?? Invalidate all tokens immediately
            await _userManager.UpdateSecurityStampAsync(user);

            return true;
        }

        public async Task<UpdateUserRolesResult> UpdateUserRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken ct = default)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
            if (user is null)
                throw new KeyNotFoundException(_Localizer[LocalizationMessages.UserNotFound]);
            var targetRoleIdStrings = roleIds?.Select(g => g.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase)
                                   ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var rolesInDb = await _context.Roles
                .Where(r => targetRoleIdStrings.Contains(r.Id))
                .Select(r => new { r.Id, r.Name })
                .ToListAsync(ct);
            var foundIds = rolesInDb.Select(r => r.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missingIds = targetRoleIdStrings.Except(foundIds, StringComparer.OrdinalIgnoreCase).ToList();
            if (missingIds.Count > 0)
                throw new ArgumentException(_Localizer[LocalizationMessages.SomeRolesNotExist] + $" {string.Join(", ", missingIds)}");
            var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var toAdd = targetRoleIdStrings.Except(currentRoleIds, StringComparer.OrdinalIgnoreCase).ToList();
            var toRemove = currentRoleIds.Except(targetRoleIdStrings, StringComparer.OrdinalIgnoreCase).ToList();
            var kept = currentRoleIds.Intersect(targetRoleIdStrings, StringComparer.OrdinalIgnoreCase).ToList();
            using var tx = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
            try
            {
                if (toRemove.Count > 0)
                {
                    var removeRows = await _context.UserRoles
                        .Where(ur => ur.UserId == userId && toRemove.Contains(ur.RoleId))
                        .ToListAsync(ct);
                    _context.UserRoles.RemoveRange(removeRows);
                }
                if (toAdd.Count > 0)
                {
                    var addRows = toAdd.Select(rid => new ApplicationUserRole
                    {
                        UserId = userId,
                        RoleId = rid
                    });
                    await _context.UserRoles.AddRangeAsync(addRows, ct);
                }
                await _context.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw; // ???? ??? ?????? ???
            }
            var finalRoles = await (from ur in _context.UserRoles.AsNoTracking()
                                    join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
                                    where ur.UserId == userId
                                    select new RoleSummaryDto { Id = r.Id, Name = r.Name! })
                                   .OrderBy(x => x.Name)
                                   .ToListAsync(ct);
            return new UpdateUserRolesResult
            {
                UserId = userId,
                AddedCount = toAdd.Count,
                RemovedCount = toRemove.Count,
                KeptCount = kept.Count,
                FinalRoles = finalRoles
            };
        }
    }
}
