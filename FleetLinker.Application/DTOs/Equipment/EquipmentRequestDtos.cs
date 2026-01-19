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
        public string RequestType { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal? RequestedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string? MaintenanceDescription { get; set; }
        public string? MaintenanceResponse { get; set; }
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
        
        public string? MaintenanceDescription { get; set; } // Specific to maintenance requests

        public string? Notes { get; set; }
    }

    public class RespondToEquipmentRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public decimal? FinalPrice { get; set; } // Optional for maintenance

        public string? MaintenanceResponse { get; set; } // Specific to maintenance responses

        public string? Notes { get; set; }

        [Required]
        public EquipmentRequestStatus Status { get; set; }
    }

    public class CreateMaintenanceRequestDto
    {
        [Required]
        public Guid EquipmentId { get; set; }

        [Required]
        public string MaintenanceDescription { get; set; } = null!;
    }

    public class RespondToMaintenanceRequestDto
    {
        [Required]
        public Guid RequestId { get; set; }

        [Required]
        public string MaintenanceResponse { get; set; } = null!;
    }
}
