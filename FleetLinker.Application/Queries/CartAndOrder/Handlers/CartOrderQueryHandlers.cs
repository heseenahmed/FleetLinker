using FleetLinker.Application.Common.Localization;
using FleetLinker.Application.DTOs;
using FleetLinker.Application.DTOs.CartAndOrder;
using FleetLinker.Application.Queries.CartAndOrder;
using FleetLinker.Domain.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FleetLinker.Application.Queries.CartAndOrder.Handlers
{
    public class CartOrderQueryHandlers :
        IRequestHandler<GetCartQuery, APIResponse<CartDto>>,
        IRequestHandler<GetOrderHistoryQuery, APIResponse<List<OrderDto>>>,
        IRequestHandler<GetOrderByIdQuery, APIResponse<OrderDto>>
    {
        private readonly ICartItemRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IAppLocalizer _localizer;

        public CartOrderQueryHandlers(
            ICartItemRepository cartRepository,
            IOrderRepository orderRepository,
            IAppLocalizer localizer)
        {
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _localizer = localizer;
        }

        public async Task<APIResponse<CartDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            var items = await _cartRepository.GetListAsync(
                predicate: c => c.UserId == request.UserId && c.IsActive,
                include: q => q.Include(c => c.SparePartOffer).ThenInclude(o => o.SparePart)
                              .Include(c => c.EquipmentRequest).ThenInclude(r => r.Equipment)
            );

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dto = new CartDto
            {
                Items = items.Select(c => new CartItemDto
                {
                    Id = c.Id,
                    SparePartOfferId = c.SparePartOfferId,
                    SparePartName = c.SparePartOffer != null 
                        ? (isArabic ? $"{c.SparePartOffer.SparePart.BrandAr} - {c.SparePartOffer.SparePart.PartNumber}" : $"{c.SparePartOffer.SparePart.BrandEn} - {c.SparePartOffer.SparePart.PartNumber}") 
                        : null,
                    EquipmentRequestId = c.EquipmentRequestId,
                    EquipmentName = c.EquipmentRequest != null 
                        ? (isArabic ? $"{c.EquipmentRequest.Equipment.BrandAr} {c.EquipmentRequest.Equipment.ModelAr}" : $"{c.EquipmentRequest.Equipment.BrandEn} {c.EquipmentRequest.Equipment.ModelEn}") 
                        : null,
                    Price = c.Price,
                    Quantity = c.Quantity
                }).ToList(),
                TotalAmount = items.Sum(c => c.Price * c.Quantity)
            };

            return APIResponse<CartDto>.Success(dto);
        }

        public async Task<APIResponse<List<OrderDto>>> Handle(GetOrderHistoryQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetListAsync(
                predicate: o => o.UserId == request.UserId && o.IsActive,
                include: q => q.Include(o => o.OrderItems)
            );

            var dtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                StatusName = o.Status.ToString(),
                CreatedDate = o.CreatedDate
            }).ToList();

            return APIResponse<List<OrderDto>>.Success(dtos);
        }

        public async Task<APIResponse<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetAsync(
                predicate: o => o.Id == request.OrderId && o.UserId == request.UserId,
                include: q => q.Include(o => o.OrderItems).ThenInclude(i => i.SparePartOffer).ThenInclude(so => so.SparePart)
                              .Include(o => o.OrderItems).ThenInclude(i => i.EquipmentRequest).ThenInclude(er => er.Equipment)
            );

            if (order == null) return APIResponse<OrderDto>.Fail(404);

            var isArabic = System.Globalization.CultureInfo.CurrentCulture.Name.StartsWith("ar");

            var dto = new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                StatusName = order.Status.ToString(),
                CreatedDate = order.CreatedDate,
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    Name = i.SparePartOffer != null 
                        ? (isArabic ? $"{i.SparePartOffer.SparePart.BrandAr} - {i.SparePartOffer.SparePart.PartNumber}" : $"{i.SparePartOffer.SparePart.BrandEn} - {i.SparePartOffer.SparePart.PartNumber}")
                        : (isArabic ? $"{i.EquipmentRequest?.Equipment.BrandAr} {i.EquipmentRequest?.Equipment.ModelAr}" : $"{i.EquipmentRequest?.Equipment.BrandEn} {i.EquipmentRequest?.Equipment.ModelEn}"),
                    Price = i.Price,
                    Quantity = i.Quantity
                }).ToList()
            };

            return APIResponse<OrderDto>.Success(dto);
        }
    }
}
