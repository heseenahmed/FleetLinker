using Benzeny.Domain.Entity.Dto;
using BenzenyMain.Domain.Entity.Dto.Company;
using MediatR;

namespace BenzenyMain.Application.Command.Company
{
    public class CreateCompanyCommand : IRequest<bool>
    {
        public CompanyForCreateDto Dto { get; }

        public CreateCompanyCommand(CompanyForCreateDto dto)
        {
            Dto = dto;
        }
    }
    public class UpdateCompanyCommand : IRequest<APIResponse<bool>>
    {
        public Guid Id { get; set; }
        public CompanyForUpdateDto CompanyDto { get; set; }
        public string UpdatedByUserId { get; }

        public UpdateCompanyCommand(Guid id, CompanyForUpdateDto dto, string updatedByUserId)
        {
            Id = id;
            CompanyDto = dto;
            UpdatedByUserId = updatedByUserId;
        }
    }

    public class SwitchActiveCompanyCommand : IRequest<APIResponse<object>>
    {
        public Guid CompanyId { get; set; }
        public string? PerformedBy { get; set; }

        public SwitchActiveCompanyCommand(Guid companyId , string? performedBy)
        {
            CompanyId = companyId;
            PerformedBy = performedBy;
        }
    }
    public class DeleteCompanyCommand : IRequest<APIResponse<object>>
    {
        public Guid CompanyId { get; }
        public string? PerformedBy { get; set; }
        public DeleteCompanyCommand(Guid companyId , string? performedBy)
        {
            CompanyId = companyId;
            PerformedBy = performedBy;
        }
    }
}
