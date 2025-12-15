
namespace Benzeny.Domain.Entity.Dto.Identity
{
    public class ApplicationUserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; } = true;
    }

}
