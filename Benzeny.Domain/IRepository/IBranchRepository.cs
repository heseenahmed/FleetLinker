
using Benzeny.Domain.Entity.Dto;
using Benzeny.Domain.IRepository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Branch;

namespace BenzenyMain.Domain.IRepository
{
    public interface IBranchRepository:IBaseRepository<Branch>
    {
        Task<bool> CreateBranchWithTransactionAsync(Branch branch, List<string>? userIds, CancellationToken cancellationToken);
        Task<PaginatedResult<BranchForListDto>> GetBranchesAsync(int pageNumber, int pageSize, string searchTerm);
        Task<BranchDetailsDto?> GetBranchByIdAsync(Guid branchId);
        Task<PaginatedResult<GetBranchsCompanyDto>> GetBranchesByCompanyAsync(
           Guid companyId, int pageNumber, int pageSize, string? searchTerm);
        Task<bool> SwitchActiveAsync(Guid branchId);
        Task<bool> DeleteBranchHardAsync(Guid branchId);
        Task<Branch?> GetByIdWithoutDeletedAsync(Guid id);
        Task<List<BranchSummaryDto>> GetBranchesForUserAsync(string userId);
        Task<bool> AssignUserToBranchAsync(string userId, Guid branchId);
        Task<bool> UnassignUserFromBranchAsync(string userId, Guid branchId);

    }
}
