
using Benzeny.Domain.Entity;

namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public class CompanyWithUserData
    {
        public Domain.Entity.Company Company { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
