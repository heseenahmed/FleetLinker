using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.CartAndOrder;
using MediatR;
using System.Collections.Generic;

namespace FleetLinker.Application.Queries.CartAndOrder
{
    public record GetCartQuery(string UserId) : IRequest<APIResponse<CartDto>>;
    public record GetOrderHistoryQuery(string UserId) : IRequest<APIResponse<List<OrderDto>>>;
    public record GetOrderByIdQuery(Guid OrderId, string UserId) : IRequest<APIResponse<OrderDto>>;
}
