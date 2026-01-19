using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using MediatR;

namespace FleetLinker.Application.Command.Equipment
{
    public class CreateEquipmentCommand : IRequest<APIResponse<object?>>
    {
        public CreateEquipmentDto Dto { get; set; }
        public string CreatedBy { get; set; }

        public CreateEquipmentCommand(CreateEquipmentDto dto, string createdBy)
        {
            Dto = dto;
            CreatedBy = createdBy;
        }
    }

    public class UpdateEquipmentCommand : IRequest<APIResponse<object?>>
    {
        public UpdateEquipmentDto Dto { get; set; }
        public string UpdatedBy { get; set; }

        public UpdateEquipmentCommand(UpdateEquipmentDto dto, string updatedBy)
        {
            Dto = dto;
            UpdatedBy = updatedBy;
        }
    }

    public class DeleteEquipmentCommand : IRequest<APIResponse<object?>>
    {
        public Guid Id { get; set; }
        public string DeletedBy { get; set; }

        public DeleteEquipmentCommand(Guid id, string deletedBy)
        {
            Id = id;
            DeletedBy = deletedBy;
        }
    }

    public class UploadEquipmentExcelCommand : IRequest<APIResponse<object?>>
    {
        public Stream FileStream { get; set; }
        public string CreatedBy { get; set; }

        public UploadEquipmentExcelCommand(Stream fileStream, string createdBy)
        {
            FileStream = fileStream;
            CreatedBy = createdBy;
        }
    }
    public class UpdateEquipmentUsageCommand : IRequest<APIResponse<object?>>
    {
        public UpdateEquipmentUsageDto Dto { get; set; }
        public string MechanicalId { get; set; }

        public UpdateEquipmentUsageCommand(UpdateEquipmentUsageDto dto, string mechanicalId)
        {
            Dto = dto;
            MechanicalId = mechanicalId;
        }
    }
}
