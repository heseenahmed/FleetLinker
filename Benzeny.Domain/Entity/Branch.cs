using Benzeny.Domain.Entity;

namespace BenzenyMain.Domain.Entity
{
    public class Branch:BaseEntity
    {
        public Guid CompanyId { get; set; }
        public string? IBAN { get; set; }
        public decimal? Balance { get; set; }
        public string? Address { get; set; }
        public bool IsMainBranch { get; set; }
        public string PhoneNumber { get; set; }
        public Guid RegionId { get; set; }
        public Guid CityId { get; set; }
        public virtual Region Region { get; set; } = null!; 
        public virtual City City { get; set; } = null!; 
        public ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
        public virtual ICollection<Car> Cars { get; set; } = new List<Car>();

        public virtual Company Company { get; set; } = null!;
    }
}
