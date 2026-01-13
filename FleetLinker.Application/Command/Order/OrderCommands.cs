using FleetLinker.Application.DTOs;
using MediatR;

namespace FleetLinker.Application.Command.Order
{
    public record CheckoutCommand(string UserId) : IRequest<APIResponse<Guid>>;
}
