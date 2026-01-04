using System.ComponentModel.DataAnnotations;
using FleetLinker.Application.Common.Localization;

namespace FleetLinker.Application.DTOs.Equipment
{
    public class EquipmentDto
    {
        public Guid Id { get; set; }
        public string Brand { get; set; } = null!;
        public int YearOfManufacture { get; set; }
        public string ChassisNumber { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string? AssetNumber { get; set; }
        public string OwnerId { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
    }

    public class CreateEquipmentDto
    {
        [Required(ErrorMessage = LocalizationMessages.BrandRequired)]
        public string Brand { get; set; } = null!;
        
        [Required(ErrorMessage = LocalizationMessages.YearRequired)]
        public int? YearOfManufacture { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.ChassisNumberRequired)]
        public string ChassisNumber { get; set; } = null!;
        
        [Required(ErrorMessage = LocalizationMessages.ModelRequired)]
        public string Model { get; set; } = null!;
        
        public string? AssetNumber { get; set; }
    }

    public class UpdateEquipmentDto
    {
        [Required(ErrorMessage = LocalizationMessages.InvalidId)]
        public Guid Id { get; set; }

        [Required(ErrorMessage = LocalizationMessages.BrandRequired)]
        public string Brand { get; set; } = null!;
        
        [Required(ErrorMessage = LocalizationMessages.YearRequired)]
        public int? YearOfManufacture { get; set; }
        
        [Required(ErrorMessage = LocalizationMessages.ChassisNumberRequired)]
        public string ChassisNumber { get; set; } = null!;
        
        [Required(ErrorMessage = LocalizationMessages.ModelRequired)]
        public string Model { get; set; } = null!;
        
        public string? AssetNumber { get; set; }
    }
}
