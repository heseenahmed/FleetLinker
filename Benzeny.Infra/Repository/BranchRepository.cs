using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Branch;
using BenzenyMain.Domain.Entity.Dto.User;
using BenzenyMain.Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BenzenyMain.Infra.Repository
{
    public class BranchRepository : BaseRepository<Branch>, IBranchRepository
    {
        private readonly ApplicationDbContext _context;

        public BranchRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CreateBranchWithTransactionAsync(Branch branch, List<string>? userIds, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                if (branch == null)
                    throw new ArgumentException("Branch data is required.");

                var companyExists = await _context.Companies.AnyAsync(c => c.Id == branch.CompanyId, cancellationToken);
                if (!companyExists)
                    throw new KeyNotFoundException("Company does not exist.");

                var regionExists = await _context.Regions.AnyAsync(r => r.Id == branch.RegionId, cancellationToken);
                if (!regionExists)
                    throw new KeyNotFoundException("Region does not exist.");

                await _context.Branches.AddAsync(branch, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                if (userIds != null && userIds.Any())
                {
                    var userBranchList = userIds.Select(userId => new UserBranch
                    {
                        UserId = userId,
                        BranchId = branch.Id
                    }).ToList();

                    await _context.UserBranchs.AddRangeAsync(userBranchList, cancellationToken);
                    await _context.SaveChangesAsync(cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);
                return true;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw; // let middleware format the real exception
            }
        }

        public async Task<PaginatedResult<BranchForListDto>> GetBranchesAsync(int pageNumber, int pageSize, string searchTerm)
        {
            var query = _context.Branches
                .AsNoTracking()
                .Select(branch => new BranchForListDto
                {
                    Id = branch.Id,
                    CompanyName = branch.Company.Name ?? "N/A",
                    IBAN = branch.IBAN ?? "N/A",
                    IsActive = branch.IsActive,
                    RegionTitle = branch.Region != null ? branch.Region.Title : "N/A",
                    CityTitle = branch.City != null ? branch.City.Name : "N/A",
                    PhoneNumber = branch.PhoneNumber,
                    Address = branch.Address,
                    RegionId = branch.RegionId,
                    CityId = branch.CityId
                });

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lowerSearch = searchTerm.ToLower();
                query = query.Where(b =>
                    b.CompanyName.ToLower().Contains(lowerSearch) ||
                    (b.IBAN != null && b.IBAN.ToLower().Contains(lowerSearch)) ||
                    (b.RegionTitle != null && b.RegionTitle.ToLower().Contains(lowerSearch)));
            }

            var totalCount = await query.CountAsync();
            var activeCount = await query.Where(x => x.IsActive).CountAsync();
            var inactiveCount = await query.Where(x => !x.IsActive).CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<BranchForListDto>(items, totalCount, pageNumber, pageSize, activeCount, inactiveCount);
        }

        public async Task<BranchDetailsDto?> GetBranchByIdAsync(Guid branchId)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            return await _context.Branches
                .AsNoTracking()
                .Where(b => b.Id == branchId)
                .Include(b => b.Company)
                .Include(b => b.Region)
                .Include(b => b.City)
                .Include(b => b.UserBranches)
                    .ThenInclude(ub => ub.User)
                        .ThenInclude(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                .Select(b => new BranchDetailsDto
                {
                    Id = b.Id,
                    CompanyName = b.Company.Name,
                    IBAN = b.IBAN,
                    IsActive = b.IsActive,
                    RegionTitle = b.Region.Title,
                    CityTitle = b.City.Name,
                    PhoneNumber = b.PhoneNumber,
                    Users = b.UserBranches
                        .Where(bu => bu.User != null)
                        .Select(bu => new UserDetailsForBranchDto
                        {
                            Id = bu.User.Id,
                            FullName = bu.User.FullName,
                            Roles = bu.User.UserRoles.Select(ur => ur.Role.Name).ToList()
                        }).ToList(),
                    Address = b.Address
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaginatedResult<GetBranchsCompanyDto>> GetBranchesByCompanyAsync(Guid companyId, int pageNumber, int pageSize, string? searchTerm)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var query = _context.Branches
                .AsNoTracking()
                .Where(b => b.CompanyId == companyId)
                .Include(b => b.Company)
                .Include(b => b.Region)
                .Select(b => new GetBranchsCompanyDto
                {
                    Id = b.Id,
                    CompanyName = b.Company.Name ?? "N/A",
                    IBAN = b.IBAN ?? "N/A",
                    IsActive = b.IsActive,
                    RegionTitle = b.Region != null ? b.Region.Title : "N/A",
                    CityTitle = b.City != null ? b.City.Name : "N/A",
                    PhoneNumber = b.PhoneNumber,
                    Address = b.Address
                });

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var lower = searchTerm.ToLower();
                query = query.Where(b =>
                    b.CompanyName.ToLower().Contains(lower) ||
                    (b.IBAN != null && b.IBAN.ToLower().Contains(lower)) ||
                    (b.RegionTitle != null && b.RegionTitle.ToLower().Contains(lower)) ||
                    (b.CityTitle != null && b.CityTitle.ToLower().Contains(lower)) ||
                    (b.PhoneNumber != null && b.PhoneNumber.ToLower().Contains(lower)));
            }

            var totalCount = await query.CountAsync();
            var totalActive = await query.Where(b => b.IsActive).CountAsync();
            var totalInactive = await query.Where(b => !b.IsActive).CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<GetBranchsCompanyDto>(items, totalCount, pageNumber, pageSize, totalActive, totalInactive);
        }

        public async Task<bool> SwitchActiveAsync(Guid branchId)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId);
            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            branch.IsActive = !branch.IsActive;
            branch.UpdatedDate = DateTime.UtcNow;
            _context.Branches.Update(branch);

            // 1) Cars in branch
            var cars = await _context.Cars.Where(c => c.BranchId == branchId).ToListAsync();
            foreach (var car in cars)
                car.IsActive = branch.IsActive;

            // 2) Drivers in branch + their users
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Where(d => d.BranchId == branchId)
                .ToListAsync();

            foreach (var driver in drivers)
            {
                driver.IsActive = branch.IsActive;
                if (driver.User != null)
                    driver.User.Active = branch.IsActive;
            }

            // 3) Users assigned only to this branch
            var userIds = await _context.UserBranchs
                .Where(ub => ub.BranchId == branchId)
                .Select(ub => ub.UserId)
                .Distinct()
                .ToListAsync();

            foreach (var userId in userIds)
            {
                var branchIdsForUser = await _context.UserBranchs
                    .Where(ub => ub.UserId == userId)
                    .Select(ub => ub.BranchId)
                    .ToListAsync();

                if (branchIdsForUser.Count == 1) // only this branch
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null)
                        user.Active = branch.IsActive;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBranchHardAsync(Guid branchId)
        {
            if (branchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var branch = await _context.Branches
                .Include(b => b.UserBranches)
                .FirstOrDefaultAsync(b => b.Id == branchId);

            if (branch == null)
                throw new KeyNotFoundException("Branch not found.");

            // 1) Users assigned to the branch
            var userIds = await _context.UserBranchs
                .Where(ub => ub.BranchId == branchId)
                .Select(ub => ub.UserId)
                .Distinct()
                .ToListAsync();

            // Remove user-branch relations
            var branchUserRelations = await _context.UserBranchs
                .Where(ub => ub.BranchId == branchId)
                .ToListAsync();
            _context.UserBranchs.RemoveRange(branchUserRelations);

            foreach (var userId in userIds)
            {
                // if user is not linked to other branches
                var branchCount = await _context.UserBranchs
                    .CountAsync(ub => ub.UserId == userId && ub.BranchId != branchId);

                if (branchCount == 0)
                {
                    // remove roles
                    var userRoles = await _context.UserRoles
                        .Where(ur => ur.UserId == userId)
                        .ToListAsync();
                    if (userRoles.Any())
                        _context.UserRoles.RemoveRange(userRoles);

                    // clear company ownership
                    var companies = await _context.Companies
                        .Where(c => c.UserId == userId)
                        .ToListAsync();
                    foreach (var company in companies)
                        company.UserId = null;

                    // remove user
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                    if (user != null)
                        _context.Users.Remove(user);
                }
            }

            // 3) Remove drivers + their users
            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Where(d => d.BranchId == branchId)
                .ToListAsync();

            foreach (var driver in drivers)
            {
                // Make sure this type matches the entity type of _context.UserRoles
                var driverUserRoles = new List<ApplicationUserRole>();

                if (driver.User != null)
                {
                    driverUserRoles = await _context.UserRoles
                        .Where(ur => ur.UserId == driver.User.Id)
                        .ToListAsync();
                }

                if (driverUserRoles.Any())
                    _context.UserRoles.RemoveRange(driverUserRoles);

                if (driver.User != null)
                    _context.Users.Remove(driver.User);

                _context.Drivers.Remove(driver);
            }


            // 4) Remove cars
            var cars = await _context.Cars.Where(c => c.BranchId == branchId).ToListAsync();
            _context.Cars.RemoveRange(cars);

            // 5) Remove branch
            _context.Branches.Remove(branch);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Branch?> GetByIdWithoutDeletedAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            return await _context.Branches.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<BranchSummaryDto>> GetBranchesForUserAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required.");

            return await _context.UserBranchs
                .AsNoTracking()
                .Where(ub => ub.UserId == userId)
                .Include(ub => ub.Branch)
                    .ThenInclude(x => x.Company)
                .Select(ub => new BranchSummaryDto
                {
                    BranchId = ub.BranchId,
                    BranchName = (ub.Branch.Company.Name + " " + ub.Branch.Address) ?? "NA"
                })
                .ToListAsync();
        }

        public async Task<bool> AssignUserToBranchAsync(string userId, Guid branchId)
        {
            if (string.IsNullOrWhiteSpace(userId) || branchId == Guid.Empty)
                throw new ArgumentException("User ID and Branch ID are required.");

            var exists = await _context.UserBranchs.AnyAsync(ub => ub.UserId == userId && ub.BranchId == branchId);
            if (exists)
                throw new ApplicationException("User is already assigned to this branch.");

            var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Id == branchId && b.IsActive);
            if (branch == null)
                throw new KeyNotFoundException("Branch not found or Not Active.");

            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId && b.Active);
            if (user == null)
                throw new KeyNotFoundException("User not found or Not Active.");

            var userBranch = new UserBranch { UserId = userId, BranchId = branchId };

            await _context.UserBranchs.AddAsync(userBranch);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UnassignUserFromBranchAsync(string userId, Guid branchId)
        {
            if (string.IsNullOrWhiteSpace(userId) || branchId == Guid.Empty)
                throw new ArgumentException("User ID and Branch ID are required.");

            var userBranch = await _context.UserBranchs.FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId);
            if (userBranch == null)
                throw new KeyNotFoundException("User is not assigned to this branch.");

            _context.UserBranchs.Remove(userBranch);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
