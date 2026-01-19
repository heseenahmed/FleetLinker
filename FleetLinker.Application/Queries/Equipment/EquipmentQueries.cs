using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using MediatR;

namespace FleetLinker.Application.Queries.Equipment
{
    public class GetAllEquipmentsQuery : IRequest<APIResponse<IEnumerable<EquipmentDto>>>
    {
        public string? Search { get; set; }

        public GetAllEquipmentsQuery(string? search = null)
        {
            Search = search;
        }
    }

    public class GetEquipmentByIdQuery : IRequest<APIResponse<EquipmentDto>>
    {
        public Guid Id { get; set; }

        public GetEquipmentByIdQuery(Guid id)
        {
            Id = id;
        }
    }

    public class DownloadEquipmentTemplateQuery : IRequest<byte[]>
    {
    }
    public class GetEquipmentsByOwnerQuery : IRequest<APIResponse<IEnumerable<EquipmentDto>>>
    {
        public string OwnerId { get; set; }

        public GetEquipmentsByOwnerQuery(string ownerId)
        {
            OwnerId = ownerId;
        }
    }
}
