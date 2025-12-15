using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity.Dto.Identity;
using FleetLinker.Domain.IRepository;
using FleetLinker.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FleetLinker.Infra.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;
        private static readonly Dictionary<string, string> RoleNameMappings = new()
        {
            { "BSuperAdmin", "Benzeny Admin" },
            { "BOperationAdmin", "Benzeny Operation Manager" },
            { "BFinanceAdmin", "Benzeny Finance Manager" },
            { "Benzeny", "Benzeny System" },
            { "CompanyOwner", "Company Owner" },
            { "BranchManager", "Branch Manager" },
            { "Driver", "Driver" },
            { "Employee", "Employee" }
        };

        public RoleRepository(RoleManager<ApplicationRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> AddRoleAsync(ApplicationRole role)
        {
            if (role is null) throw new ArgumentNullException(nameof(role));
            if (string.IsNullOrWhiteSpace(role.Name)) throw new ArgumentException("Role name is required.", nameof(role));

            // idempotent: if role already exists, treat as success (preserves current behavior)
            if (IsRoleExistsAsync(role.Name)) return true;

            role.NormalizedName = role.Name.ToUpperInvariant();

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                throw new ApplicationException(
                    "Failed to create role: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<bool> DeleteRoleByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Role id is required.", nameof(id));

            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
                throw new ApplicationException(
                    "Failed to delete role: " + string.Join(", ", result.Errors.Select(e => e.Description)));

            return true;
        }

        public async Task<bool> DeleteRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Role name is required.", nameof(roleName));

            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Remove related user-role links first
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.RoleId == role.Id)
                    .ToListAsync();

                if (userRoles.Count > 0)
                {
                    _context.UserRoles.RemoveRange(userRoles);
                    await _context.SaveChangesAsync();
                }

                var result = await _roleManager.DeleteAsync(role);
                if (!result.Succeeded)
                    throw new ApplicationException(
                        "Failed to delete role: " + string.Join(", ", result.Errors.Select(e => e.Description)));

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw; // let middleware handle
            }
        }

        public async Task<ApplicationRole?> GetRoleByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Role id is required.", nameof(id));
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<ApplicationRole?> GetRoleByNameAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Role name is required.", nameof(roleName));
            return await _roleManager.FindByNameAsync(roleName);
        }

        public async Task<IEnumerable<ApplicationRole>> GetRolesAsync()
        {
            var roles = await _roleManager.Roles
                                .AsNoTracking()
                                .OrderByDescending(r => r.Id)
                                .ToListAsync();

            // Map DB names to friendly names
            foreach (var role in roles)
            {
                if (RoleNameMappings.TryGetValue(role.Name ?? string.Empty, out var friendlyName))
                {
                    role.Name = friendlyName;
                }
            }

            return roles;
        }

        // Kept public signature/behavior as in your codebase (despite the Async suffix, it’s sync).
        public bool IsRoleExistsAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) throw new ArgumentException("Role name is required.", nameof(roleName));
            return _roleManager.Roles.Any(e => e.Name != null && e.Name.ToLower() == roleName.ToLower());
        }

        public async Task<bool> UpdateRoleAsync(RoleDto roleDto)
        {
            if (roleDto is null) throw new ArgumentNullException(nameof(roleDto));
            if (string.IsNullOrWhiteSpace(roleDto.Name)) throw new ArgumentException("Role name is required.", nameof(roleDto.Name));

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name != null && r.Name.ToLower() == roleDto.Name.ToLower());

            if (role == null) throw new KeyNotFoundException("Role not found.");

            role.Name = roleDto.Name;
            role.NormalizedName = roleDto.Name.ToUpperInvariant();

            var changed = await _context.SaveChangesAsync();
            return changed > 0;
        }
    }
}
