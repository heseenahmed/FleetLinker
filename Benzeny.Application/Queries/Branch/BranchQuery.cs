
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Branch;
using MediatR;

namespace BenzenyMain.Application.Queries.Branch
{
    public record GetBranchList(int PageNumber, int PageSize, string SearchTerm) : IRequest<APIResponse<PaginatedResult<BranchForListDto>>>;
    public class GetBranchById : IRequest<APIResponse<BranchDetailsDto>>
    {
        public Guid BranchId { get; set; }

        public GetBranchById(Guid branchId)
        {
            BranchId = branchId;
        }
    }
    public class GetBranchsInCompany : IRequest<APIResponse<PaginatedResult<GetBranchsCompanyDto>>>
    {
        public Guid CompanyId { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }

        public GetBranchsInCompany(Guid companyId, int pageNumber, int pageSize, string? searchTerm)
        {
            CompanyId = companyId;
            PageNumber = pageNumber;
            PageSize = pageSize;
            SearchTerm = searchTerm;
        }
    }

}
