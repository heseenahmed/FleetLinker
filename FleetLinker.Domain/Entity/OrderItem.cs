using FleetLinker.Domain.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; } = null!;

        public Guid? SparePartOfferId { get; set; }
        [ForeignKey("SparePartOfferId")]
        public virtual SparePartOffer? SparePartOffer { get; set; }

        public Guid? EquipmentRequestId { get; set; }
        [ForeignKey("EquipmentRequestId")]
        public virtual EquipmentRequest? EquipmentRequest { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
