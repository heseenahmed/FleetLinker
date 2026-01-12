using FleetLinker.Application.DTOs.SparePartOffer;
using FleetLinker.Application.DTOs;
using MediatR;
using FleetLinker.Application.DTOs;
using System.Collections.Generic;

namespace FleetLinker.Application.Queries.SparePartOffer
{
    public record GetSupplierSparePartOffersQuery(string SupplierId) : IRequest<APIResponse<List<SparePartOfferDto>>>;
}
