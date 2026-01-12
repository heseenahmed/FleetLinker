using FleetLinker.Application.Command.SparePartOffer;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FleetLinker.Application.Command.SparePartOffer.Handlers
{
    public class RespondToSparePartOfferCommandHandler : IRequestHandler<RespondToSparePartOfferCommand, APIResponse<bool>>
    {
        private readonly ISparePartOfferRepository _offerRepository;
        private readonly IEquipmentSparePartRepository _sparePartRepository;
        private readonly IAppLocalizer _localizer;

        public RespondToSparePartOfferCommandHandler(
            ISparePartOfferRepository offerRepository,
            IEquipmentSparePartRepository sparePartRepository,
            IAppLocalizer localizer)
        {
            _offerRepository = offerRepository;
            _sparePartRepository = sparePartRepository;
            _localizer = localizer;
        }

        public async Task<APIResponse<bool>> Handle(RespondToSparePartOfferCommand request, CancellationToken cancellationToken)
        {
            var offer = await _offerRepository.GetAsync(
                predicate: x => x.Id == request.Dto.OfferId,
                include: q => q.Include(o => o.SparePart)
            );

            if (offer == null)
            {
                return APIResponse<bool>.Fail(404, message: _localizer[LocalizationMessages.SparePartNotFound]);
            }

            if (offer.SupplierId != request.SupplierId)
            {
                return APIResponse<bool>.Fail(403, message: _localizer[LocalizationMessages.Unauthorized]);
            }

            // Option 1: Update the offer price
            offer.FinalPrice = request.Dto.FinalPrice;
            offer.Notes = request.Dto.Notes;
            offer.Status = OfferStatus.Responded;
            offer.UpdatedDate = DateTime.UtcNow;
            offer.UpdatedBy = request.SupplierId;

            // Option 2: If requested, update the system price for future offers
            if (request.Dto.SetAsSystemPrice && offer.SparePart != null)
            {
                offer.SparePart.Price = request.Dto.FinalPrice;
                offer.SparePart.UpdatedBy = request.SupplierId;
                offer.SparePart.UpdatedDate = DateTime.UtcNow;
                await _sparePartRepository.UpdateAsync(offer.SparePart);
            }

            await _offerRepository.UpdateAsync(offer);

            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.OfferRespondedSuccessfully]);
        }
    }
}
