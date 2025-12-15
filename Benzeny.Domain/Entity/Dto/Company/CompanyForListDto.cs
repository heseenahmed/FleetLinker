
namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public class CompanyForListDto
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public List<string>? FilePaths { get; set; }
        public string IBAN { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? UserName { get; set; }
        public string? Fullname { get; set; }
        public string? Ssn { get; set; }
        public string? Phonenumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        //public string? Refresh { get; set; }
        //public string? AccessToken { get; set; }
    }
}
