
using Benzeny.Domain.Entity;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Company;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace BenzenyMain.Domain.IRepository
{
    public interface ICompanyRepository
    {
        Task<Company> AddAsync(Company company, CancellationToken cancellationToken);
        Task<Company?> GetByIdAsync(Guid companyId);
        Task<CompanyWithUserData?> GetCompanyWithUserByIdAsync(Guid companyId);

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Task<bool> UpdateCompanyAsync(Company company);
        Task<PaginatedResult<Company>> GetPaginatedListAsync( Expression<Func<Company, bool>>? predicate,
                                                            int pageNumber,
                                                            int pageSize,
                                                            Expression<Func<Company, bool>>? searchExpression = null);
        Task<List<ApplicationUser>> GetAllUsersInCompanyAsync(Guid companyId, int pageNumber, int pageSize, string? searchTerm);
        Task<(int Total, int Active, int Inactive)> GetUserStatsInCompanyAsync(Guid companyId, string? searchTerm);
        Task<ApplicationUser?> GetUserByIdInCompanyAsync(Guid companyId, string userId);
        Task<bool> SwitchActiveCompanyAsync(Guid companyId);
        Task<bool> DeleteCompanyAsync(Guid companyId);
        Task<int> GetCompanyCountByActivation(Expression<Func<Company, bool>>? predicate);
        Task<List<CompanyCsvRowDto>> GetAllForCsvAsync(CancellationToken ct = default);
        // جديدة: نفس الإسقاط ولكن مخصّصة لمسار الـ PDF (للفصل الواضح)
        Task<List<CompanyCsvRowDto>> GetAllForPdfAsync(CancellationToken ct = default);

    }
}
