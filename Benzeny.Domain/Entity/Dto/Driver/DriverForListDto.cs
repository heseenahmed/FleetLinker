
namespace BenzenyMain.Domain.Entity.Dto.Driver
{
    public class DriverForListDto
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid? BranchId { get; set; }
        public Guid? CarId { get; set; }
        public string? FullName { get; set; } = null!;
        public string? TagName { get; set; }
        public string? BranchName { get; set; } = null!;
        public string? License { get; set; }
        public string? LicenseDegree { get; set; }
        public string? ConsumptionType { get; set; }
        public string? Balance { get; set; }
        public bool CardStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string? CarPlate { get; set; }
        public bool IsActive { get; set; }
    }
}
