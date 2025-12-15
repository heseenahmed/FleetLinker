
using Benzeny.Domain.Entity;
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity
{
    public class Company:BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string? ProfilePicturePath { get; set; }
        public List<string>? FilePaths { get; set; }
        public string? UserId { get; set; }
        public string? IBAN { get; set; }
        public ICollection<Branch> Branches { get; set; } = new List<Branch>();
        public ApplicationUser? CompanyOwner { get; set; }
        public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    }
}
