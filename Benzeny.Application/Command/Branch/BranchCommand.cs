
using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Branch;
using MediatR;

namespace BenzenyMain.Application.Command.Branch
{
    public class CreateBranchCommand : IRequest<APIResponse<bool>>
    {
        public BranchForCreateDto BranchDto { get; set; }

        public CreateBranchCommand(BranchForCreateDto branchDto)
        {
            BranchDto = branchDto;
        }
    }
    public class SwitchActiveCommand : IRequest<APIResponse<bool>>
    {
        public Guid BranchId { get; }

        public SwitchActiveCommand(Guid branchId)
        {
            BranchId = branchId;
        }
    }
    public class DeleteBranchCommand : IRequest<APIResponse<bool>>
    {
        public Guid BranchId { get; }
        public DeleteBranchCommand(Guid branchId)
        {
            BranchId = branchId;
        }
    }
    public class UpdateBranchCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public BranchForUpdateDto BranchDto { get; set; }

        public UpdateBranchCommand(Guid id, BranchForUpdateDto dto)
        {
            Id = id;
            BranchDto = dto;
        }
    }
    public class AssignUserToBranchCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public Guid BranchId { get; set; }

        public AssignUserToBranchCommand(string userId, Guid branchId)
        {
            UserId = userId;
            BranchId = branchId;
        }
    }
    public class UnassignUserFromBranchCommand : IRequest<bool>
    {
        public string UserId { get; set; }
        public Guid BranchId { get; set; }

        public UnassignUserFromBranchCommand(string userId, Guid branchId)
        {
            UserId = userId;
            BranchId = branchId;
        }
    }

}
