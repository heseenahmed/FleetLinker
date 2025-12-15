using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public class CompanyDto
    {
        public Guid? Id { get; set; } = new Guid();
        public string Name { get; set; }
        public string Description { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string IBAN { get; set; }
        public List<string>? FilePaths { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
