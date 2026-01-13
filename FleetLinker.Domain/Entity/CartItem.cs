using FleetLinker.Domain.Entity;
using FleetLinker.Domain.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class CartItem : BaseEntity
    {
        public string UserId { get; set; } = null!;
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;

        public Guid? SparePartOfferId { get; set; }
        [ForeignKey("SparePartOfferId")]
        public virtual SparePartOffer? SparePartOffer { get; set; }

        public Guid? EquipmentRequestId { get; set; }
        [ForeignKey("EquipmentRequestId")]
        public virtual EquipmentRequest? EquipmentRequest { get; set; }

        public decimal Price { get; set; }
        public int Quantity { get; set; } = 1;

        // Helps identify which type of item it is easily
        public bool IsEquipmentRequest => EquipmentRequestId.HasValue;
        public bool IsSparePartOffer => SparePartOfferId.HasValue;
    }
}
