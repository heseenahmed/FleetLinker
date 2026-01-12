using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class SparePartOffer : BaseEntity
    {
        [Required]
        public Guid SparePartId { get; set; }

        [ForeignKey("SparePartId")]
        public virtual EquipmentSparePart SparePart { get; set; } = null!;

        [Required]
        public string RequesterId { get; set; } = null!; // Visitor or Equipment Owner User ID

        [ForeignKey("RequesterId")]
        public virtual ApplicationUser Requester { get; set; } = null!;

        [Required]
        public string SupplierId { get; set; } = null!; // Supplier User ID

        [ForeignKey("SupplierId")]
        public virtual ApplicationUser Supplier { get; set; } = null!;

        [Required]
        public Enums.OfferStatus Status { get; set; } = Enums.OfferStatus.Pending;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? FinalPrice { get; set; }
        
        public string? Notes { get; set; }
    }
}
