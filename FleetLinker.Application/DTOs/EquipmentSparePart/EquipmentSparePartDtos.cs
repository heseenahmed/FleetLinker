using FleetLinker.Domain.Enums;
using Microsoft.AspNetCore.Http;
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
        public string? Manufacturer { get; set; }
        public string? ImagePath { get; set; }
        public decimal Price { get; set; }
        public bool IsPriceHidden { get; set; }
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

        public string? Manufacturer { get; set; }
        
        public IFormFile? ImageFile { get; set; }

        public decimal Price { get; set; }

        public bool IsPriceHidden { get; set; }
    }

    public class UpdateEquipmentSparePartDto : CreateEquipmentSparePartDto
    {
        [Required]
        public Guid Id { get; set; }
    }
}
