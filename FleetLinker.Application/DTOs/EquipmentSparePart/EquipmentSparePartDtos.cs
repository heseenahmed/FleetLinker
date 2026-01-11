using FleetLinker.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Application.DTOs.EquipmentSparePart
{
    public class EquipmentSparePartDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = null!; // Localized Type Name
        public string TypeAr { get; set; } = null!;
        public string TypeEn { get; set; } = null!;
        public string PartNumber { get; set; } = null!;
        public string Brand { get; set; } = null!; // Localized Brand
        public string? BrandAr { get; set; }
        public string? BrandEn { get; set; }
        public int YearOfManufacture { get; set; }
        public string? AssetNumber { get; set; }
        public string SupplierId { get; set; } = null!;
        public string SupplierName { get; set; } = null!;
    }

    public class CreateEquipmentSparePartDto
    {
        [Required]
        public PartType Type { get; set; }

        [Required]
        public string PartNumber { get; set; } = null!;

        public string? BrandAr { get; set; }
        public string? BrandEn { get; set; }

        [Required]
        public int YearOfManufacture { get; set; }

        public string? AssetNumber { get; set; }
    }

    public class UpdateEquipmentSparePartDto : CreateEquipmentSparePartDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
