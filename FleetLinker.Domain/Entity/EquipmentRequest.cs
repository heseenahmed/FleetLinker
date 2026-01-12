using FleetLinker.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FleetLinker.Domain.Entity
{
    public class EquipmentRequest : BaseEntity
    {
        [Required]
        public Guid EquipmentId { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; } = null!;

        [Required]
        public string RequesterId { get; set; } = null!;

        [ForeignKey("RequesterId")]
        public virtual ApplicationUser Requester { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = null!;

        [ForeignKey("OwnerId")]
        public virtual ApplicationUser Owner { get; set; } = null!;

        [Required]
        public EquipmentRequestType RequestType { get; set; }

        [Required]
        public EquipmentRequestStatus Status { get; set; }

        public decimal? RequestedPrice { get; set; } // Price suggested by the requester

        public decimal? FinalPrice { get; set; } // Price offered by the owner

        public string? Notes { get; set; }
    }
}
