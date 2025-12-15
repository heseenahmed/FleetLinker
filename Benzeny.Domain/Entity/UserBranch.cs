
using Benzeny.Domain.Entity;

namespace BenzenyMain.Domain.Entity
{
    public class UserBranch
    {
        public string UserId { get; set; } = null!;
        public Guid BranchId { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public Branch Branch { get; set; } = null!;
    }
}
