using FleetLinker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Application.DTOs.Equipment
{
    public class EquipmentRequestDto
    {
        public Guid Id { get; set; }
        public Guid EquipmentId { get; set; }
        public string EquipmentBrand { get; set; } = null!;
        public string EquipmentModel { get; set; } = null!;
        public string RequesterId { get; set; } = null!;
        public string RequesterName { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public EquipmentRequestType RequestType { get; set; }
        public string RequestTypeName { get; set; } = null!;
        public EquipmentRequestStatus Status { get; set; }
        public string StatusName { get; set; } = null!;
        public decimal? RequestedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateEquipmentRequestDto
    {
        [Required]
        public Guid EquipmentId { get; set; }

        [Required]
        public EquipmentRequestType RequestType { get; set; }

        public decimal? RequestedPrice { get; set; }

        public string? Notes { get; set; }
    }

    public class RespondToEquipmentRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public decimal FinalPrice { get; set; }

        public string? Notes { get; set; }

        [Required]
        public EquipmentRequestStatus Status { get; set; } // Accepted, Rejected, or Responded
    }
}
