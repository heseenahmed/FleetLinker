using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using Benzeny.Infra.Data;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Company;
using BenzenyMain.Domain.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace BenzenyMain.Infra.Repository
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company> AddAsync(Company company, CancellationToken cancellationToken)
        {
            if (company == null)
                throw new ArgumentException("Company data must be provided.");

            _context.Companies.Add(company);
            await _context.SaveChangesAsync(cancellationToken);
            return company;
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
        {
            return await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task<Company?> GetByIdAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            return await _context.Companies
                .Include(x=>x.Users)
                    .ThenInclude(x=>x.UserRoles)
                        .ThenInclude(x=>x.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);
        }

        public async Task<CompanyWithUserData?> GetCompanyWithUserByIdAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var company = await _context.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                return null;

            ApplicationUser? user = null;
            if (!string.IsNullOrEmpty(company.UserId))
            {
                user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == company.UserId);
            }

            return new CompanyWithUserData
            {
                Company = company,
                User = user
            };
        }

        public async Task<bool> UpdateCompanyAsync(Company company)
        {
            if (company == null)
                throw new ArgumentException("Company data is required.");

            _context.Companies.Update(company);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<PaginatedResult<Company>> GetPaginatedListAsync(
            Expression<Func<Company, bool>>? predicate,
            int pageNumber,
            int pageSize,
            Expression<Func<Company, bool>>? searchExpression = null)
        {
            IQueryable<Company> query = _context.Companies.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            if (searchExpression != null)
                query = query.Where(searchExpression);

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.UpdatedDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<Company>(items, total, pageNumber, pageSize);
        }

        public async Task<List<ApplicationUser>> GetAllUsersInCompanyAsync(Guid companyId, int pageNumber, int pageSize, string? searchTerm)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            IQueryable<ApplicationUser> query = _context.Users
                .AsNoTracking()
                .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                .Where(u => u.CompanyId == companyId && !u.Deleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    EF.Functions.Like(u.FullName, $"%{searchTerm}%") ||
                    EF.Functions.Like(u.Email, $"%{searchTerm}%"));
            }

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<(int Total, int Active, int Inactive)> GetUserStatsInCompanyAsync(Guid companyId, string? searchTerm)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            IQueryable<ApplicationUser> query = _context.Users
                .AsNoTracking()
                .Where(u => u.CompanyId == companyId && !u.Deleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    EF.Functions.Like(u.FullName, $"%{searchTerm}%") ||
                    EF.Functions.Like(u.Email, $"%{searchTerm}%"));
            }

            int total = await query.CountAsync();
            int active = await query.CountAsync(u => u.Active);
            int inactive = await query.CountAsync(u => !u.Active);

            return (total, active, inactive);
        }

        public async Task<ApplicationUser?> GetUserByIdInCompanyAsync(Guid companyId, string userId)
        {
            if (companyId == Guid.Empty || string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("Company ID and User ID are required.");

            return await _context.Users
                .AsNoTracking()
                .Include(u => u.UserRoles)
                    .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(u =>
                    u.Id == userId &&
                    u.CompanyId == companyId &&
                    !u.Deleted);
        }

        public async Task<bool> SwitchActiveCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var company = await _context.Companies
                .Include(c => c.Branches)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            company.IsActive = !company.IsActive;

            // For each branch under the company, flip activation and cascade to related entities
            foreach (var branch in company.Branches)
            {
                branch.IsActive = company.IsActive;

                // Users assigned to the branch (many-to-many)
                var userBranches = await _context.UserBranchs
                    .Include(ub => ub.User)
                    .Where(ub => ub.BranchId == branch.Id && ub.User != null && !ub.User.Deleted)
                    .ToListAsync();

                foreach (var ub in userBranches)
                {
                    if (ub.User != null)
                        ub.User.Active = company.IsActive;
                }

                // Cars in branch
                var cars = await _context.Cars
                    .Where(c => c.BranchId == branch.Id)
                    .ToListAsync();

                foreach (var car in cars)
                    car.IsActive = company.IsActive;

                // Drivers in branch (+ their users)
                var drivers = await _context.Drivers
                    .Include(d => d.User)
                    .Where(d => d.BranchId == branch.Id)
                    .ToListAsync();

                foreach (var driver in drivers)
                {
                    driver.IsActive = company.IsActive;
                    if (driver.User != null)
                        driver.User.Active = company.IsActive;
                }
            }
            await _context.SaveChangesAsync();
            return company.IsActive ? true : false;
        }

        public async Task<bool> DeleteCompanyAsync(Guid companyId)
        {
            if (companyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            var company = await _context.Companies
                .Include(c => c.Branches)
                .FirstOrDefaultAsync(c => c.Id == companyId);

            if (company == null)
                throw new KeyNotFoundException("Company not found.");

            // 1) detach company owner if present (avoid circular refs)
            if (!string.IsNullOrEmpty(company.UserId))
            {
                company.UserId = null;
                _context.Companies.Update(company);
                await _context.SaveChangesAsync();
            }

            // 2) purge each branch’s content
            foreach (var branch in company.Branches)
            {
                // cars
                var cars = await _context.Cars
                    .Where(c => c.BranchId == branch.Id)
                    .ToListAsync();
                _context.Cars.RemoveRange(cars);

                // drivers + their users
                var drivers = await _context.Drivers
                    .Include(d => d.User)
                    .Where(d => d.BranchId == branch.Id)
                    .ToListAsync();

                foreach (var driver in drivers)
                {
                    if (driver.User != null)
                    {
                        var userRoles = await _context.UserRoles
                            .Where(r => r.UserId == driver.User.Id)
                            .ToListAsync();

                        if (userRoles.Any())
                            _context.UserRoles.RemoveRange(userRoles);

                        _context.Users.Remove(driver.User);
                    }

                    _context.Drivers.Remove(driver);
                }

                // user-branch relations
                var userBranches = await _context.UserBranchs
                    .Where(ub => ub.BranchId == branch.Id)
                    .ToListAsync();

                if (userBranches.Any())
                    _context.UserBranchs.RemoveRange(userBranches);
            }

            // 3) users directly tied to company
            var companyUsers = await _context.Users
                .Where(u => u.CompanyId == companyId)
                .ToListAsync();

            foreach (var user in companyUsers)
            {
                var userRoles = await _context.UserRoles
                    .Where(r => r.UserId == user.Id)
                    .ToListAsync();

                if (userRoles.Any())
                    _context.UserRoles.RemoveRange(userRoles);

                _context.Users.Remove(user);
            }

            // 4) branches
            _context.Branches.RemoveRange(company.Branches);

            // 5) company
            _context.Companies.Remove(company);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetCompanyCountByActivation(Expression<Func<Company, bool>>? predicate)
        {
            return predicate != null
                ? await _context.Companies.AsNoTracking().Where(predicate).CountAsync()
                : await _context.Companies.AsNoTracking().CountAsync();
        }
        public async Task<List<CompanyCsvRowDto>> GetAllForCsvAsync(CancellationToken ct = default)
           => await ProjectForExport().ToListAsync(ct);

        public async Task<List<CompanyCsvRowDto>> GetAllForPdfAsync(CancellationToken ct = default)
            => await ProjectForExport().ToListAsync(ct);
        private IQueryable<CompanyCsvRowDto> ProjectForExport()
        {
            return _context.Companies
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new CompanyCsvRowDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CompanyEmail = c.CompanyEmail,
                    CompanyPhone = c.CompanyPhone,
                    IBAN = c.IBAN,
                    ProfilePicturePath = c.ProfilePicturePath,
                    FilesCount = c.FilePaths != null ? c.FilePaths.Count : 0,
                    OwnerUserId = c.UserId,
                    BranchesCount = c.Branches.Count,
                    UsersCount = c.Users.Count,
                    CreatedDate = c.CreatedDate,
                    UpdatedDate = c.UpdatedDate
                });
        }
    }
}
