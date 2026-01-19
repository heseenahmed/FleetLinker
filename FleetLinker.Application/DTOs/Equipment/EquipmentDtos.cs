using FleetLinker.Application.Common.Localization;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Application.DTOs.Equipment
{
    public class EquipmentDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = null!;
        public string? BrandAr { get; set; }
        public string BrandEn { get; set; } = null!;
        public int YearOfManufacture { get; set; }
        public string ChassisNumber { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? ModelAr { get; set; }
        public string ModelEn { get; set; } = null!;
        public string? AssetNumber { get; set; }
        public string OwnerId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public bool ForSale { get; set; }
        public bool ForRent { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RentPrice { get; set; }
        public string? ImagePath { get; set; }
        public string? Description { get; set; }
        public decimal? UsageHours { get; set; }
        public decimal? FuelLiters { get; set; }
        public string? MechanicalId { get; set; }
        public string? MechanicalName { get; set; }
    }

    public class CreateEquipmentDto
    {
        public string? BrandAr { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.BrandRequired)]
        public string BrandEn { get; set; } = null!;
        
        [Required(ErrorMessage = LocalizationMessages.YearRequired)]
        public int? YearOfManufacture { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.ChassisNumberRequired)]
        public string ChassisNumber { get; set; } = null!;
        
        public string? ModelAr { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.ModelRequired)]
        public string ModelEn { get; set; } = null!;
        
        public string? AssetNumber { get; set; }

        public bool ForSale { get; set; }
        public bool ForRent { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RentPrice { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class UpdateEquipmentDto
    {
        [Required(ErrorMessage = LocalizationMessages.InvalidId)]
        public Guid Id { get; set; }

        public string? BrandAr { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.BrandRequired)]
        public string BrandEn { get; set; } = null!;
        
        [Required(ErrorMessage = LocalizationMessages.YearRequired)]
        public int? YearOfManufacture { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.ChassisNumberRequired)]
        public string ChassisNumber { get; set; } = null!;
        
        public string? ModelAr { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.ModelRequired)]
        public string ModelEn { get; set; } = null!;
        
        public string? AssetNumber { get; set; }

        public bool ForSale { get; set; }
        public bool ForRent { get; set; }
        public decimal? SalePrice { get; set; }
        public decimal? RentPrice { get; set; }
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
    public class UpdateEquipmentUsageDto
    {
        [Required(ErrorMessage = LocalizationMessages.InvalidId)]
        public Guid EquipmentId { get; set; }

        [Required(ErrorMessage = LocalizationMessages.UsageHoursRequired)]
        public decimal UsageHours { get; set; }

        [Required(ErrorMessage = LocalizationMessages.FuelLitersRequired)]
        public decimal FuelLiters { get; set; }
    }
}
