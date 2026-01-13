using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.CartAndOrder;
using MediatR;

namespace FleetLinker.Application.Command.Cart
{
    public record AddToCartCommand(AddToCartDto Dto, string UserId) : IRequest<APIResponse<bool>>;
    public record RemoveFromCartCommand(Guid CartItemId, string UserId) : IRequest<APIResponse<bool>>;
    public record ClearCartCommand(string UserId) : IRequest<APIResponse<bool>>;
}
