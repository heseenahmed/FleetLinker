using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.EquipmentSparePart;
using MediatR;

namespace FleetLinker.Application.Queries.EquipmentSparePart
{
    public class GetAllEquipmentSparePartsQuery : IRequest<APIResponse<IEnumerable<EquipmentSparePartDto>>>
    {
        public string? Search { get; set; }
        public GetAllEquipmentSparePartsQuery(string? search = null) => Search = search;
    }

    public class GetEquipmentSparePartByIdQuery : IRequest<APIResponse<EquipmentSparePartDto>>
    {
        public Guid Id { get; set; }
        public GetEquipmentSparePartByIdQuery(Guid id) => Id = id;
    }
}
