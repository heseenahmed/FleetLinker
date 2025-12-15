
namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public class GetUserDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public List<string>? UserRoles { get; set; }
        public bool IsActive { get; set; }
    }
}
