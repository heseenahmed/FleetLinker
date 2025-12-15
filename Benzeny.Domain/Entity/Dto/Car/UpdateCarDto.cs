
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Car
{
    public class UpdateCarDto
    {
        public Guid Id { get; set; }
        public int? CarModelId { get; set; }
        public int? CarBrandId { get; set; }
        public int? PlateTypeId { get; set; }
        public int? CarTypeId { get; set; }
        public bool IsActive { get; set; } = true;
        public string? CarNumber { get; set; } = null!;
        public string? Color { get; set; }
        public string? CardNum { get; set; }
        public int? Petroltype { get; set; }
        public DateTime? LicenseDate { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedDate { get; set; }
        [JsonIgnore]
        public string? UpdatedBy { get; set; }
    }
}
