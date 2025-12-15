using AutoMapper;
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.IRepository;
using MediatR;

namespace BenzenyMain.Application.Command.Branch
{
    public class BranchCommandHandler :
        IRequestHandler<CreateBranchCommand, APIResponse<bool>>,
        IRequestHandler<SwitchActiveCommand, APIResponse<bool>>,
        IRequestHandler<DeleteBranchCommand, APIResponse<bool>>,
        IRequestHandler<UpdateBranchCommand, bool>,
        IRequestHandler<AssignUserToBranchCommand, bool>,
        IRequestHandler<UnassignUserFromBranchCommand, bool>
    {
        private readonly IBranchRepository _branchRepository;
        private readonly IMapper _mapper;

        public BranchCommandHandler(IBranchRepository branchRepository, IMapper mapper)
        {
            _branchRepository = branchRepository;
            _mapper = mapper;
        }

        public async Task<APIResponse<bool>> Handle(CreateBranchCommand request, CancellationToken cancellationToken)
        {
            var dto = request.BranchDto
                      ?? throw new ArgumentException("Branch data is required.");

            if (string.IsNullOrWhiteSpace(dto.Address) || dto.RegionId == Guid.Empty)
                throw new ArgumentException("Branch data is incomplete or invalid.");

            var branch = _mapper.Map<Domain.Entity.Branch>(dto);
            branch.Balance = 0;
            branch.CreatedDate = DateTime.UtcNow;
            branch.IBAN = string.Empty;
            branch.IsActive = false;
            branch.CreatedBy = dto.CreatedBy ?? "Admin";
            branch.CreatedDate= DateTime.UtcNow;
            var created = await _branchRepository.CreateBranchWithTransactionAsync(branch, dto.UserIds, cancellationToken);
            if (!created)
                throw new ApplicationException("Branch creation failed due to internal error.");

            return APIResponse<bool>.Success(true, "Branch created successfully.");
        }

        public async Task<APIResponse<bool>> Handle(SwitchActiveCommand request, CancellationToken cancellationToken)
        {
            var success = await _branchRepository.SwitchActiveAsync(request.BranchId);
            if (!success)
                throw new KeyNotFoundException("Branch not found or could not be updated.");

            return APIResponse<bool>.Success(true, "Branch and users updated successfully.");
        }

        public async Task<APIResponse<bool>> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
        {
            var success = await _branchRepository.DeleteBranchHardAsync(request.BranchId);
            if (!success)
                throw new KeyNotFoundException("Branch not found or already deleted.");

            return APIResponse<bool>.Success(true, "Branch and related users deleted successfully.");
        }

        public async Task<bool> Handle(UpdateBranchCommand request, CancellationToken cancellationToken)
        {
            var dto = request.BranchDto ?? throw new ArgumentException("Branch update data is required.");

            var branch = await _branchRepository.GetByIdWithoutDeletedAsync(request.Id)
                         ?? throw new KeyNotFoundException("Branch not found.");

            branch.UpdatedDate = DateTime.UtcNow;
            branch.UpdatedBy = dto.UpdatedBy ?? branch.UpdatedBy;
            branch.Address = dto.Address ?? branch.Address;
            branch.RegionId = dto.RegionId;
            branch.UpdatedBy= dto.UpdatedBy ?? "Admin";
            branch.UpdatedDate= DateTime.UtcNow;
            await _branchRepository.UpdateAsync(branch);
            return true;
        }

        public async Task<bool> Handle(AssignUserToBranchCommand request, CancellationToken cancellationToken)
        {
            // Return raw result; controller decides how to message false.
            return await _branchRepository.AssignUserToBranchAsync(request.UserId, request.BranchId);
        }

        public async Task<bool> Handle(UnassignUserFromBranchCommand request, CancellationToken cancellationToken)
        {
            // Return raw result; controller decides how to message false.
            return await _branchRepository.UnassignUserFromBranchAsync(request.UserId, request.BranchId);
        }
    }
}
