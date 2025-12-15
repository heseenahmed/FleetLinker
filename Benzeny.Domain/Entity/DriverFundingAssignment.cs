
using Benzeny.Domain.Entity;
using BenzenyMain.Domain.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenzenyMain.Domain.Entity
{
    public class DriverFundingAssignment : BaseEntity
    {
        public Guid DriverId { get; set; }
        [ForeignKey(nameof(DriverId))]
        public Driver Driver { get; set; } = null!;

        public decimal Amount { get; set; }                    // assigned amount
        public TransactionType TransactionType { get; set; }   // OneTime or Limit
        public LimitType? LimitType { get; set; }              // Day / Weekly / Monthly when Limit

        // For Weekly: store selected days in a single int column (flags)
        public WeekDays? WeeklyDays { get; set; }              // null unless Weekly

        // For Monthly: the day-of-month captured when assignment is created
        public byte? MonthlyDay { get; set; }                  // 1..31

        public DateTime NextRunAtUtc { get; set; }             // when to apply next
        public DateTime? LastRunAtUtc { get; set; }
    }
}
