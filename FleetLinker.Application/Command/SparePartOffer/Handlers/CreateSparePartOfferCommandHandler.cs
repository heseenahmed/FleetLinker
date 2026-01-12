using FleetLinker.Application.Command.SparePartOffer;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace FleetLinker.Application.Command.SparePartOffer.Handlers
{
    public class CreateSparePartOfferCommandHandler : IRequestHandler<CreateSparePartOfferCommand, APIResponse<bool>>
    {
        private readonly ISparePartOfferRepository _offerRepository;
        private readonly IEquipmentSparePartRepository _sparePartRepository;
        private readonly IAppLocalizer _localizer;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateSparePartOfferCommandHandler(
            ISparePartOfferRepository offerRepository,
            IEquipmentSparePartRepository sparePartRepository,
            IAppLocalizer localizer,
            UserManager<ApplicationUser> userManager)
        {
            _offerRepository = offerRepository;
            _sparePartRepository = sparePartRepository;
            _localizer = localizer;
            _userManager = userManager;
        }

        public async Task<APIResponse<bool>> Handle(CreateSparePartOfferCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.RequesterId);
            if (user == null)
            {
                return APIResponse<bool>.Fail(401, message: _localizer[LocalizationMessages.Unauthorized]);
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Visitor") && !roles.Contains("Equipment owner"))
            {
                return APIResponse<bool>.Fail(403, message: _localizer[LocalizationMessages.Unauthorized]);
            }

            var sparePart = await _sparePartRepository.GetByGuidAsync(request.Dto.SparePartId);
            if (sparePart == null)
            {
                return APIResponse<bool>.Fail(404, message: _localizer[LocalizationMessages.SparePartNotFound]);
            }

            var offer = new FleetLinker.Domain.Entity.SparePartOffer
            {
                Id = Guid.NewGuid(),
                SparePartId = request.Dto.SparePartId,
                RequesterId = request.RequesterId,
                SupplierId = sparePart.SupplierId,
                Status = OfferStatus.Pending,
                Notes = request.Dto.Notes,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = request.RequesterId,
                IsActive = true
            };

            // Automatic Pricing Logic
            if (sparePart.Price > 0 && !sparePart.IsPriceHidden)
            {
                offer.FinalPrice = sparePart.Price;
                offer.Status = OfferStatus.Responded;
                offer.Notes = string.IsNullOrWhiteSpace(offer.Notes) 
                    ? "Automatic price applied" 
                    : offer.Notes + " (Automatic price applied)";
            }

            await _offerRepository.AddAsync(offer);
            
            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.OfferCreatedSuccessfully]);
        }
    }
}
