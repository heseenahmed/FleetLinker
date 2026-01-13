using FleetLinker.Application.Command.Cart;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Command.Cart.Handlers
{
    public class CartCommandHandlers :
        IRequestHandler<AddToCartCommand, APIResponse<bool>>,
        IRequestHandler<RemoveFromCartCommand, APIResponse<bool>>,
        IRequestHandler<ClearCartCommand, APIResponse<bool>>
    {
        private readonly ICartItemRepository _cartRepository;
        private readonly ISparePartOfferRepository _offerRepository;
        private readonly IEquipmentRequestRepository _equipmentRequestRepository;
        private readonly IAppLocalizer _localizer;

        public CartCommandHandlers(
            ICartItemRepository cartRepository,
            ISparePartOfferRepository offerRepository,
            IEquipmentRequestRepository equipmentRequestRepository,
            IAppLocalizer localizer)
        {
            _cartRepository = cartRepository;
            _offerRepository = offerRepository;
            _equipmentRequestRepository = equipmentRequestRepository;
            _localizer = localizer;
        }

        public async Task<APIResponse<bool>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
        {
            decimal price = 0;

            if (request.Dto.SparePartOfferId.HasValue)
            {
                var offer = await _offerRepository.GetByGuidAsync(request.Dto.SparePartOfferId.Value);
                if (offer == null || offer.Status != OfferStatus.Responded || offer.RequesterId != request.UserId)
                    return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.InvalidCartItem]);

                // Check if already in cart
                var existing = await _cartRepository.GetAsync(c => c.UserId == request.UserId && c.SparePartOfferId == request.Dto.SparePartOfferId);
                if (existing != null) return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.ItemAlreadyInCart]);
                
                price = offer.FinalPrice ?? 0;
            }
            else if (request.Dto.EquipmentRequestId.HasValue)
            {
                var equipReq = await _equipmentRequestRepository.GetByGuidAsync(request.Dto.EquipmentRequestId.Value);
                if (equipReq == null || (equipReq.Status != EquipmentRequestStatus.Responded && equipReq.Status != EquipmentRequestStatus.Accepted) || equipReq.RequesterId != request.UserId)
                    return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.InvalidCartItem]);

                // Check if already in cart
                var existing = await _cartRepository.GetAsync(c => c.UserId == request.UserId && c.EquipmentRequestId == request.Dto.EquipmentRequestId);
                if (existing != null) return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.ItemAlreadyInCart]);

                price = equipReq.FinalPrice ?? 0;
            }
            else
            {
                return APIResponse<bool>.Fail(400, message: _localizer[LocalizationMessages.InvalidCartItem]);
            }

            var cartItem = new CartItem
            {
                UserId = request.UserId,
                SparePartOfferId = request.Dto.SparePartOfferId,
                EquipmentRequestId = request.Dto.EquipmentRequestId,
                Price = price,
                Quantity = request.Dto.Quantity,
                CreatedBy = request.UserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            await _cartRepository.AddAsync(cartItem);
            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.ItemAddedToCart]);
        }

        public async Task<APIResponse<bool>> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
        {
            var item = await _cartRepository.GetAsync(c => c.Id == request.CartItemId && c.UserId == request.UserId);
            if (item == null) return APIResponse<bool>.Fail(404, message: _localizer[LocalizationMessages.NotFound]);

            await _cartRepository.RemoveAsync(item);
            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.ItemRemovedFromCart]);
        }

        public async Task<APIResponse<bool>> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var items = await _cartRepository.GetListAsync(c => c.UserId == request.UserId);
            foreach (var item in items)
            {
                await _cartRepository.RemoveAsync(item);
            }
            return APIResponse<bool>.Success(true, _localizer[LocalizationMessages.CartCleared]);
        }
    }
}
