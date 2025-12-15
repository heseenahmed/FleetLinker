
namespace BenzenyMain.Domain.Entity.Dto.User
{
    public class UserDetailsForBranchDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
    }
}
