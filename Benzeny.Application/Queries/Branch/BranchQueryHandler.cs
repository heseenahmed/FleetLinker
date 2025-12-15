using AutoMapper;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Branch;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Queries.Branch
{
    public class BranchQueryHandler :
        IRequestHandler<GetBranchList, APIResponse<PaginatedResult<BranchForListDto>>>,
        IRequestHandler<GetBranchById, APIResponse<BranchDetailsDto>>,
        IRequestHandler<GetBranchsInCompany, APIResponse<PaginatedResult<GetBranchsCompanyDto>>>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public BranchQueryHandler(IBranchRepository branchRepository, IMapper mapper)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
        }

        public async Task<APIResponse<PaginatedResult<BranchForListDto>>> Handle(GetBranchList request, CancellationToken cancellationToken)
        {
            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new ArgumentException("Invalid pagination parameters.");

            var result = await _branchRepository.GetBranchesAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm ?? string.Empty);

            return APIResponse<PaginatedResult<BranchForListDto>>.Success(result, "Branches retrieved successfully.");
        }

        public async Task<APIResponse<BranchDetailsDto>> Handle(GetBranchById request, CancellationToken cancellationToken)
        {
            if (request.BranchId == Guid.Empty)
                throw new ArgumentException("Invalid branch ID.");

            var branch = await _branchRepository.GetBranchByIdAsync(request.BranchId)
                         ?? throw new KeyNotFoundException("Branch not found.");

            var dto = _mapper.Map<BranchDetailsDto>(branch);
            dto.CompanyName = branch.CompanyName ?? "N/A";
            dto.IBAN ??= "N/A";

            return APIResponse<BranchDetailsDto>.Success(dto, "Branch retrieved successfully.");
        }

        public async Task<APIResponse<PaginatedResult<GetBranchsCompanyDto>>> Handle(GetBranchsInCompany request, CancellationToken cancellationToken)
        {
            if (request.CompanyId == Guid.Empty)
                throw new ArgumentException("Invalid company ID.");

            if (request.PageNumber <= 0 || request.PageSize <= 0)
                throw new ArgumentException("Invalid pagination parameters.");

            var result = await _branchRepository.GetBranchesByCompanyAsync(
                request.CompanyId,
                request.PageNumber,
                request.PageSize,
                request.SearchTerm ?? string.Empty);

            return APIResponse<PaginatedResult<GetBranchsCompanyDto>>.Success(result, "Branches retrieved successfully.");
        }
    }
}
