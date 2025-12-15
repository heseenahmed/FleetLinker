using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto.Identity;
using Benzeny.Domain.IRepository;
using Benzeny.Infra.Data;
using BenzenyMain.Application.Common;
using BenzenyMain.Domain.Entity.Dto.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Benzeny.Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
           )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserInfoAPI?> GetUserInfoAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("User ID is required.", nameof(id));

            try
            {
                return await _context.Users
                    .Where(e => e.Id == id && e.Active && !e.Deleted)
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
                throw new ApplicationException("Error while retrieving user information.", ex);
            }
        }

        public async Task<bool> UpdateUserAsync(UserForUpdateDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto), "User update payload is required.");
            if (string.IsNullOrWhiteSpace(dto.Id))
                throw new ArgumentException("User ID is required.", nameof(dto.Id));

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.FindByIdAsync(dto.Id);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                if (!string.IsNullOrWhiteSpace(dto.Email))
                {
                    var emailExists = await _userManager.Users
                        .AnyAsync(u => u.Id != dto.Id &&
                                       u.Email == dto.Email &&
                                       !u.Deleted);
                    if (emailExists)
                        throw new ArgumentException("Email already in use by another user.");
                }

                if (!string.IsNullOrWhiteSpace(dto.Mobile))
                {
                    var phoneExists = await _userManager.Users
                        .AnyAsync(u => u.Id != dto.Id &&
                                       u.PhoneNumber == dto.Mobile &&
                                       !u.Deleted);
                    if (phoneExists)
                        throw new ArgumentException("Mobile number already in use by another user.");
                }

                //if (dto.CompanyId.HasValue)
                //{
                //    var companyExists = await _context.Companies
                //        .AnyAsync(b => b.Id == dto.CompanyId.Value);
                //    if (!companyExists)
                //        throw new KeyNotFoundException("Company not found.");
                //}

                user.FullName = dto.FullName ?? user.FullName;
                user.Email = dto.Email ?? user.Email;
                user.PhoneNumber = dto.Mobile ?? user.PhoneNumber;
                user.UserName = dto.Username ?? user.UserName;
                user.EmailConfirmed = true;
                user.Active = true;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                    throw new ApplicationException("Failed to update user: " +
                        string.Join(", ", updateResult.Errors.Select(e => e.Description)));

                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, dto.Password);
                    if (!passwordResult.Succeeded)
                        throw new ApplicationException("Failed to reset password: " +
                            string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
                }

                // Update roles
                var oldRoles = await _userManager.GetRolesAsync(user);
                if (oldRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, oldRoles);
                    if (!removeResult.Succeeded)
                        throw new ApplicationException("Failed to remove existing roles: " +
                            string.Join(", ", removeResult.Errors.Select(e => e.Description)));
                }

                if (dto.RoleIds?.Any() == true)
                {
                    var roleNames = await _roleManager.Roles
                        .Where(r => dto.RoleIds.Contains(r.Id))
                        .Select(r => r.Name!)
                        .ToListAsync();

                    if (!roleNames.Any())
                        throw new ArgumentException("Invalid roles provided.");

                    var addResult = await _userManager.AddToRolesAsync(user, roleNames);
                    if (!addResult.Succeeded)
                        throw new ApplicationException("Failed to assign roles: " +
                            string.Join(", ", addResult.Errors.Select(e => e.Description)));
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
                throw new ArgumentNullException(nameof(request), "Registration payload is required.");
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required.");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required.");
            if (string.IsNullOrWhiteSpace(request.Mobile))
                throw new ArgumentException("Mobile is required.");

            var normalizedEmail = request.Email.Trim().ToUpperInvariant();

            var existingUsers = await _context.Users
                .Where(u => u.NormalizedEmail == normalizedEmail && !u.Deleted)
                .ToListAsync();

            if (existingUsers.Count > 1)
                throw new InvalidOperationException("Multiple users found with the same email.");
            if (existingUsers.Count == 1)
                throw new ArgumentException("Email is already in use.");

            Guid? resolvedCompanyId = request.CompanyId;

            //if (request.BranchId.HasValue)
            //{
            //    var branch = await _context.Branches
            //        .FirstOrDefaultAsync(b => b.Id == request.BranchId.Value);
            //    if (branch == null)
            //        throw new KeyNotFoundException("Branch not found.");
            //    resolvedCompanyId = branch.CompanyId;
            //}

            //if (request.CompanyId.HasValue)
            //{
            //    var companyExists = await _context.Companies
            //        .AnyAsync(c => c.Id == request.CompanyId.Value);
            //    if (!companyExists)
            //        throw new KeyNotFoundException("Company not found.");
            //}

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
                Active = false,
                Deleted = false,
                CompanyId = resolvedCompanyId,
                RefreshToken = refreshToken,
                RefreshTokenExpiryUTC = DateTime.UtcNow.AddDays(7),
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
                throw new ApplicationException("User creation failed: " +
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));

            //if (request.BranchId.HasValue)
            //{
            //    await _context.UserBranchs.AddAsync(new UserBranch
            //    {
            //        UserId = user.Id,
            //        BranchId = request.BranchId.Value
            //    });
            //    await _context.SaveChangesAsync();
            //}

            if (request.RoleIds?.Any() == true)
            {
                foreach (var roleId in request.RoleIds)
                {
                    var role = await _roleManager.FindByIdAsync(roleId.ToString());
                    if (role == null)
                        throw new KeyNotFoundException("Role not found.");
                    var addRoleResult = await _userManager.AddToRoleAsync(user, role.Name!);
                    if (!addRoleResult.Succeeded)
                        throw new ApplicationException("Failed to assign role: " +
                            string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
                }
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var password = !string.IsNullOrWhiteSpace(request.Password)
                ? request.Password
                : request.Mobile;

            var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
            if (!passwordResult.Succeeded)
                throw new ApplicationException("Password setup failed: " +
                    string.Join(", ", passwordResult.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<bool> SwitchUserActiveAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("User ID is required.", nameof(id));

            var user = await _userManager.Users.FirstOrDefaultAsync(e => e.Id == id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.Active = !user.Active;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user.Active;
        }

        public async Task<ApplicationUser?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("User ID is required.", nameof(id));

            try
            {
                return await _context.Users
                    .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving user by ID.", ex);
            }
        }

        public async Task<List<ApplicationUser>> GetAllAsync()
        {
            try
            {
                return await _context.Users
                    .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .Where(x => !x.Deleted)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving all users.", ex);
            }
        }

        public async Task<GetUsersInCompany> GetAllUsersInCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Company ID is required.", nameof(companyId));

            try
            {
                //var companyExists = await _context.Companies.AnyAsync(b => b.Id == companyId);
                //if (!companyExists)
                //    throw new KeyNotFoundException("Company not found.");

                var users = await _context.Users
                    .Where(u => !u.Deleted)
                    .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
                    .ToListAsync();

                return new GetUsersInCompany
                {
                    TotalCount = users.Count,
                    ActiveCount = users.Count(u => u.Active),
                    InActiveCount = users.Count(u => !u.Active),
                    Users = users.Select(u => new UserForListDto
                    {
                        Id = u.Id,
                        FullName = u.FullName,
                        Email = u.Email,
                        Mobile = u.PhoneNumber,
                        CompanyId = u.CompanyId,
                        UserRoles = u.UserRoles.Select(r => r.Role.Name!).ToList(),
                        IsActive = u.Active
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error retrieving users for company.", ex);
            }
        }

        public async Task<int> CountAdminsAsync()
        {
            try
            {
                return await _context.Users
                    .Include(x => x.UserRoles)
                    .CountAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Admin"));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while counting Admins.", ex);
            }
        }

        public async Task<bool> SoftDeleteUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.", nameof(userId));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.Deleted)
                throw new KeyNotFoundException("User not found or already deleted.");

            user.Deleted = true;
            user.Active = false;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new ApplicationException("Failed to soft delete user: " +
                    string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }
        public async Task<(List<UserBenzenyDto> , int Count , int ActiveCount , int InActiveCount)> GetAllBenzenyUsersAsync(CancellationToken ct = default)
        {
            var rename = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["BOperationAdmin"] = "Operation",
                ["BSuperAdmin"] = "SuperAdmin",
                ["BFinanceAdmin"] = "Finance",
                // أضف غيرها لو لزم
            };
            string[] extraRoles = { "Benzeny", "BSuperAdmin", "BOperationAdmin", "BFinanceAdmin" };

            var rolesQuery = _context.Roles
                                   .AsNoTracking()
                                   .Where(r => extraRoles.Contains(r.Name));

            //var query =
            //    from u in _context.Users.AsNoTracking() 
            //    from ur in u.UserRoles 
            //    join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
            //    where extraRoles.Contains(r.Name)
            //    select new { u, RoleName = r.Name! };
            var userQuery = _context.Users.Where(x=>x.Deleted==false).AsNoTracking()
                .Where(u => u.UserRoles.Any(ur =>
                    _context.Roles.Where(r => extraRoles.Contains(r.Name))
                                  .Select(r => r.Id)
                                  .Contains(ur.RoleId)));

            var count = await userQuery.CountAsync(ct);
            var activeCount = await userQuery.CountAsync(u => u.Active, ct);
            var inactiveCount = await userQuery.CountAsync(u => !u.Active, ct);

            var result = await userQuery
                .Select(u => new UserBenzenyDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    IsActive = u.Active,
                    Deleted = u.Deleted,
                    BirthDay = u.BirthDay,
                    Roles = u.UserRoles
                        .Where(ur => _context.Roles.Where(r => extraRoles.Contains(r.Name))
                                                   .Select(r => r.Id)
                                                   .Contains(ur.RoleId))
                        .Select(ur => ur.Role!.Name!)
                        .Distinct()
                        .ToList()
                })
                .OrderBy(x => x.FullName)
                .ToListAsync(ct);
            result.ForEach(u =>
            {
                u.Roles = (u.Roles ?? new List<string>())
                    .Select(r => rename.TryGetValue(r, out var m) ? m : r)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s)
                    .ToList();
            });
            return (result , count , activeCount , inactiveCount);
        }
        public async Task<UpdateUserRolesResult> UpdateUserRolesAsync(string userId, IEnumerable<Guid> roleIds, CancellationToken ct = default)
        {
            // 1) تحقق من وجود المستخدم
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user is null)
                throw new KeyNotFoundException($"User not found: {userId}");

            // 2) حوّل الـ Guids إلى string (Identity uses string keys by default)
            var targetRoleIdStrings = roleIds?.Select(g => g.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase)
                                   ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // 3) هات كل الأدوار المطلوبة من الـ DB وتأكد إنها موجودة
            var rolesInDb = await _context.Roles
                .Where(r => targetRoleIdStrings.Contains(r.Id))
                .Select(r => new { r.Id, r.Name })
                .ToListAsync(ct);

            var foundIds = rolesInDb.Select(r => r.Id).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var missingIds = targetRoleIdStrings.Except(foundIds, StringComparer.OrdinalIgnoreCase).ToList();
            if (missingIds.Count > 0)
                throw new ArgumentException($"Some roles do not exist: {string.Join(", ", missingIds)}");

            // 4) حساب القوائم: الحالي، المطلوب، الإضافة/الإزالة/الثبات
            var currentRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var toAdd = targetRoleIdStrings.Except(currentRoleIds, StringComparer.OrdinalIgnoreCase).ToList();
            var toRemove = currentRoleIds.Except(targetRoleIdStrings, StringComparer.OrdinalIgnoreCase).ToList();
            var kept = currentRoleIds.Intersect(targetRoleIdStrings, StringComparer.OrdinalIgnoreCase).ToList();

            // 5) تنفيذ داخل ترانزاكشن بسيطة
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
                throw; // هيمر على الميدل وير
            }

            // 6) رجّع النتيجة النهائية
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
