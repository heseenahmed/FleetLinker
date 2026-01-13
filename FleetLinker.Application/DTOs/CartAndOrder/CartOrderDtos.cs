using FleetLinker.Domain.Enums;
using System.Collections.Generic;

namespace FleetLinker.Application.DTOs.CartAndOrder
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid? SparePartOfferId { get; set; }
        public string? SparePartName { get; set; }
        public Guid? EquipmentRequestId { get; set; }
        public string? EquipmentName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class CartDto
    {
        public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
        public decimal TotalAmount { get; set; }
    }

    public class AddToCartDto
    {
        public Guid? SparePartOfferId { get; set; }
        public Guid? EquipmentRequestId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    public class OrderDto
    {
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
        public DateTime CreatedDate { get; set; }
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    public class OrderItemDto
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
