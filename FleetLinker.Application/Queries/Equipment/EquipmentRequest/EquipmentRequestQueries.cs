using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.Equipment;
using MediatR;
using System.Collections.Generic;

namespace FleetLinker.Application.Queries.Equipment.EquipmentRequest
{
    public record GetOwnerEquipmentRequestsQuery(string OwnerId) : IRequest<APIResponse<List<EquipmentRequestDto>>>;

    public record GetRequesterEquipmentRequestsQuery(string RequesterId) : IRequest<APIResponse<List<EquipmentRequestDto>>>;
}
