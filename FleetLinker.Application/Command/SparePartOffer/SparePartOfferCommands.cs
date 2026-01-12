using FleetLinker.Application.DTOs.SparePartOffer;
using FleetLinker.Application.DTOs;
using MediatR;
using FleetLinker.Application.DTOs;

namespace FleetLinker.Application.Command.SparePartOffer
{
    public record CreateSparePartOfferCommand(CreateSparePartOfferDto Dto, string RequesterId) : IRequest<APIResponse<bool>>;
    public record RespondToSparePartOfferCommand(RespondToSparePartOfferDto Dto, string SupplierId) : IRequest<APIResponse<bool>>;
}
