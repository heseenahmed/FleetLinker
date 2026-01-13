using FleetLinker.Application.Command.Order;
using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Command.Order.Handlers
{
    public class OrderCommandHandlers :
        IRequestHandler<CheckoutCommand, APIResponse<Guid>>
    {
        private readonly ICartItemRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAppLocalizer _localizer;

        public OrderCommandHandlers(
            ICartItemRepository cartRepository,
            IOrderRepository orderRepository,
            IAppLocalizer localizer)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
        }

        public async Task<APIResponse<Guid>> Handle(CheckoutCommand request, CancellationToken cancellationToken)
        {
            var cartItems = await _cartRepository.GetListAsync(c => c.UserId == request.UserId && c.IsActive);
            if (!cartItems.Any())
                return APIResponse<Guid>.Fail(400, message: _localizer[LocalizationMessages.InsufficientCart]);

            var orderId = Guid.NewGuid();
            var totalAmount = cartItems.Sum(c => c.Price * c.Quantity);

            var order = new Domain.Entity.Order
            {
                Id = orderId,
                UserId = request.UserId,
                TotalAmount = totalAmount,
                Status = OrderStatus.PendingPayment,
                CreatedBy = request.UserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    SparePartOfferId = c.SparePartOfferId,
                    EquipmentRequestId = c.EquipmentRequestId,
                    Price = c.Price,
                    Quantity = c.Quantity,
                    CreatedBy = request.UserId,
                    CreatedDate = DateTime.UtcNow,
                    IsActive = true
                }).ToList()
            };

            await _orderRepository.AddAsync(order);

            // Clear Cart
            foreach (var item in cartItems)
            {
                await _cartRepository.RemoveAsync(item);
            }

            return APIResponse<Guid>.Success(orderId, _localizer[LocalizationMessages.OrderCreatedSuccessfully]);
        }
    }
}
