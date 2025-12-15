
namespace BenzenyMain.Domain.Entity.Dto.Car
{
    public class CarForListDto
    {
        public string Model { get; set; } = null!;
        public string CarNumber { get; set; } = null!;
        public string? Color { get; set; }
        public string? CardNum { get; set; }
        public DateTime? LicenseDate { get; set; }
        public string? CarModel { get; set; }
        public int? CarModelId { get; set; }
        public string? CarBrand { get; set; }
        public int? CarBrandId { get; set; }
        public string? PlateType { get; set; }
        public int? PlateTypeId { get; set; }
        public string? CarType { get; set; }
        public int? CarTypeId { get; set; }
        public Guid BranchId { get; set; }
        public string BranchTitle { get; set; } = null!;
        public List<string> DriversName { get; set; }
        public int? PetrolType { get; set; }

        public bool IsActive { get; set; }

    }
}
