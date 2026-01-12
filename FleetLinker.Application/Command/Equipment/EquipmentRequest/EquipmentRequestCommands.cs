using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using MediatR;

namespace FleetLinker.Application.Command.Equipment.EquipmentRequest
{
    public record CreateEquipmentRequestCommand(CreateEquipmentRequestDto Dto, string RequesterId) : IRequest<APIResponse<bool>>;
    
    public record RespondToEquipmentRequestCommand(RespondToEquipmentRequestDto Dto, string OwnerId) : IRequest<APIResponse<bool>>;
}
