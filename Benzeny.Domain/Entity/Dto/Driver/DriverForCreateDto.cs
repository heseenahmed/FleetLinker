
namespace BenzenyMain.Domain.Entity.Dto.Driver
{
    public class DriverForCreateDto
    {
        public string FullName { get; set; } = null!;
        public string phoneNumber { get; set; } = null!;
        public Guid branchId { get; set; }
        public int TagId { get; set; }
        public string? License { get; set; }
        public string? LicenseDegree { get; set; }
        public string? CreatedBy { get; set; } = null!;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
