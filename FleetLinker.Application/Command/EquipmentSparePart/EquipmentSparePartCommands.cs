using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.EquipmentSparePart;
using MediatR;

namespace FleetLinker.Application.Command.EquipmentSparePart
{
    public record CreateEquipmentSparePartCommand(CreateEquipmentSparePartDto Dto, string SupplierId) : IRequest<APIResponse<object?>>;
    public record UpdateEquipmentSparePartCommand(UpdateEquipmentSparePartDto Dto, string SupplierId) : IRequest<APIResponse<object?>>;
    public record DeleteEquipmentSparePartCommand(Guid Id, string SupplierId) : IRequest<APIResponse<object?>>;
}
