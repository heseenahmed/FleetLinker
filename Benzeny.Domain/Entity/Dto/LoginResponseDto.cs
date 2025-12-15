
using Benzeny.Domain.Entity.Dto.User;
using BenzenyMain.Domain.Entity.Dto.Branch;

namespace Benzeny.Domain.Entity.Dto
{
    public class LoginResponseDto
    {
        public string UserId { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public string AccessToken { get; set; } = null!;
        public Guid? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public bool FirstTimeLogin { get; set; }
        public List<BranchSummaryDto> Branches { get; set; } = new();
        public List<string> Roles { get; set; } = new();
    }
}
