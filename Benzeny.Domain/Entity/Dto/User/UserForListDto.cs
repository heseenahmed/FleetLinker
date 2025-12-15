using System.ComponentModel.DataAnnotations;

namespace Benzeny.Domain.Entity.Dto.Identity
{
    public class UserForListDto
    {
        public string Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public List<string>? UserRoles { get; set; }
        public string? BranchName { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? CompanyId { get; set; }
        public bool IsActive { get; set; }

    }
}
