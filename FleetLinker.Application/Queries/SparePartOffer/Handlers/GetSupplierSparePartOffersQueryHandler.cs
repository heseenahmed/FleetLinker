using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.SparePartOffer;
using FleetLinker.Application.Queries.SparePartOffer;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Queries.SparePartOffer.Handlers
{
    public class GetSupplierSparePartOffersQueryHandler : IRequestHandler<GetSupplierSparePartOffersQuery, APIResponse<List<SparePartOfferDto>>>
    {
        private readonly ISparePartOfferRepository _offerRepository;
        private readonly IAppLocalizer _localizer;

        public GetSupplierSparePartOffersQueryHandler(ISparePartOfferRepository offerRepository, IAppLocalizer localizer)
        {
            _offerRepository = offerRepository;
            _localizer = localizer;
        }

        public async Task<APIResponse<List<SparePartOfferDto>>> Handle(GetSupplierSparePartOffersQuery request, CancellationToken cancellationToken)
        {
            var offers = await _offerRepository.GetListAsync(
                predicate: x => x.SupplierId == request.SupplierId && x.IsActive,
                include: q => q.Include(o => o.SparePart).Include(o => o.Requester)
            );
            
            var dtos = offers.Select(o => new SparePartOfferDto
            {
                Id = o.Id,
                SparePartId = o.SparePartId,
                SparePartName = o.SparePart?.PartNumber ?? "N/A",
                RequesterId = o.RequesterId,
                RequesterName = o.Requester?.FullName ?? "N/A",
                SupplierId = o.SupplierId,
                Status = o.Status,
                StatusName = o.Status.ToString(),
                FinalPrice = o.FinalPrice,
                SystemPrice = o.SparePart?.Price ?? 0,
                Notes = o.Notes,
                CreatedDate = o.CreatedDate
            }).ToList();

            return APIResponse<List<SparePartOfferDto>>.Success(dtos, _localizer[LocalizationMessages.OffersRetrievedSuccessfully]);
        }
    }
}
