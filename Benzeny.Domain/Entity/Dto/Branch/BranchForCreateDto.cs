
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Branch
{
    public class BranchForCreateDto
    {
        public Guid CompanyId { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public List<string> UserIds { get; set; } = new();
        public Guid RegionId { get; set; } // ✅ Added RegionId
        public Guid CityId { get; set; }
        [JsonIgnore]
        public string? CreatedBy { get; set; }
    }
}
