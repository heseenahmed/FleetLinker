
using Azure;
using Benzeny.Domain.Entity;
using BenzenyMain.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity
{
    public class Driver : BaseEntity
    {
        public Guid BranchId { get; set; }
        public string UserId { get; set; } = null!;
        public string? License { get; set; }
        public string? LicenseDegree { get; set; }
        //public TransactionType? type { get; set; }
        //public decimal? Amount { get; set; }
        public decimal? Balance { get; set; }
        public bool CardStatus { get; set; } = false;
        public int? TagId { get; set; }
        public Tag? Tag { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; } = null!;
        public virtual Branch Branch { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<CarDriver> CarDrivers { get; set; } = new List<CarDriver>();
        public virtual ICollection<DriverFundingAssignment> FundingAssignments { get; set; } = new List<DriverFundingAssignment>();

    }
}
