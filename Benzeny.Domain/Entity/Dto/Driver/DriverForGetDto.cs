
namespace BenzenyMain.Domain.Entity.Dto.Driver
{
    public class DriverForGetDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string TagTitle { get; set; }
        public string? License { get; set; }
        public string? LicenseDegree { get; set; }
    }
}
