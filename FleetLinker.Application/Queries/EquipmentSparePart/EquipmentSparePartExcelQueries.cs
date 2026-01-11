using MediatR;
using FleetLinker.Application.DTOs;

namespace FleetLinker.Application.Queries.EquipmentSparePart
{
    public record DownloadSparePartTemplateQuery() : IRequest<APIResponse<byte[]>>;
}
