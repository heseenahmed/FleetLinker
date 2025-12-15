
using Benzeny.Domain.Entity.Dto.Identity;

namespace BenzenyMain.Domain.Entity.Dto.User
{
    public class GetUsersInCompany
    {
        public int ActiveCount { get; set; }
        public int InActiveCount { get; set; }
        public int TotalCount { get; set; }
        public List<UserForListDto> Users { get; set; }
    }
}
