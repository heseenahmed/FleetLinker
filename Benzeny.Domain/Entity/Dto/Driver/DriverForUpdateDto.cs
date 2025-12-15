
namespace BenzenyMain.Domain.Entity.Dto.Driver
{
    public class DriverForUpdateDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; } = null!;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? phoneNumber { get; set; } = null!;
        public Guid branchId { get; set; }
        public int TagId { get; set; }
        public string? License { get; set; }
        public string? LicenseDegree { get; set; }
        public string UpdatedBy { get; set; } = null!;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
