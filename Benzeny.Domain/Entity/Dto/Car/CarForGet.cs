
namespace BenzenyMain.Domain.Entity.Dto.Car
{
    public class CarForGet
    {
        public Guid Id { get; set; }
        public string CarNumber { get; set; }
        public string Color { get; set; }
        public string? CarModel { get; set; }
        public string? CarBrand { get; set; }
        public string? PlateType { get; set; }
        public string? CarType { get; set; }
        public List<string> DriversName { get; set; }
        public DateTime? LicenseDate { get; set; }
        public int? PetrolType { get; set; }
        public bool IsActive { get; set; }
    }
}
