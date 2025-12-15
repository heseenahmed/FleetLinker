
namespace BenzenyMain.Domain.Entity.Dto.Car
{
    public class CarForCreateDto
    {
        public int? CarModelId { get; set; }
        public int? CarBrandId { get; set; }
        public int? PlateTypeId { get; set; }
        public int? CarTypeId { get; set; }
        public string CarNumber { get; set; } = null!;
        public string? Color { get; set; }
        public string? CardNum { get; set; }
        public DateTime? LicenseDate { get; set; }
        public Guid BranchId { get; set; }
        public int? PetrolType { get; set; }
        public string? CreatedBy { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
