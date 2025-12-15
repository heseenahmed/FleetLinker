
using BenzenyMain.Domain.Entity.Dto.User;

namespace BenzenyMain.Domain.Entity.Dto.Branch
{
    public class BranchDetailsDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? IBAN { get; set; }
        public bool IsActive { get; set; }
        public Guid RegionId { get; set; }
        public string? RegionTitle { get; set; }
        public Guid CityId { get; set; }
        public string? CityTitle { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public List<UserDetailsForBranchDto> Users { get; set; }
    }
}
