using MediatR;
using Microsoft.AspNetCore.Http;
using FleetLinker.Application.DTOs;

namespace FleetLinker.Application.Command.EquipmentSparePart
{
    public record BatchUploadSparePartsCommand(IFormFile File, string SupplierId) : IRequest<APIResponse<bool>>;
}
