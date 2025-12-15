
namespace BenzenyMain.Domain.Entity.Dto.User
{
    public sealed class UserBenzenyDto
    {
        public string Id { get; set; } = default!;
        public string FullName { get; set; } = default!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool Deleted { get; set; }
        public DateTime BirthDay { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
