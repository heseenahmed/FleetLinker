
using Benzeny.Domain.Entity;
using System.Diagnostics.Eventing.Reader;

namespace BenzenyMain.Domain.Entity
{
    public class Car : BaseEntity
    {
        public Guid BranchId { get; set; }
        public string CarNumber { get; set; }
        public string? Color { get; set; }
        public int? Petroltype { get; set; }
        public string? CardNum { get; set; }
        public DateTime? LicenseDate { get; set; }
        public int? CarModelId { get; set; }
        public int? CarBrandId { get; set; }
        public int? PlateTypeId { get; set; }
        public int? CarTypeId { get; set; }
        public CarModel? CarModel { get; set; }
        public CarType? CarType { get; set; }
        public PlateType? PlateType { get; set; }
        public CarBrand? CarBrand { get; set; }
        public virtual Branch Branch { get; set; } = null!;
        public virtual ICollection<CarDriver> CarDrivers { get; set; } = new List<CarDriver>();

    }
}
