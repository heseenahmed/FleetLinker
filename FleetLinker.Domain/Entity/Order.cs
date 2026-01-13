using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }
        public string? PaymentTransactionId { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
