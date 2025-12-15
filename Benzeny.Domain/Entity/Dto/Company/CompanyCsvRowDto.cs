
namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public sealed class CompanyCsvRowDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string CompanyEmail { get; set; } = default!;
        public string CompanyPhone { get; set; } = default!;
        public string? IBAN { get; set; }
        public string? ProfilePicturePath { get; set; }
        public int FilesCount { get; set; }
        public string? OwnerUserId { get; set; } // من Company.UserId
        public int BranchesCount { get; set; }
        public int UsersCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
