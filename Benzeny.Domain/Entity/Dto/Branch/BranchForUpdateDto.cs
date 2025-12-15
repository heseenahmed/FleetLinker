
using System.ComponentModel.DataAnnotations;

namespace BenzenyMain.Domain.Entity.Dto.Branch
{
    public class BranchForUpdateDto
    {
        public string? Address { get; set; }
        public Guid RegionId { get; set; }
        public Guid CityId { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
