
using BenzenyMain.Domain.Enum;

namespace BenzenyMain.Domain.Entity.Dto.Driver
{
    public class DriverFundingAssignmentDto
    {
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public LimitType? LimitType { get; set; }
        public List<Days>? Days { get; set; }            // Decoded from flags
        public byte? MonthlyDay { get; set; }
    }
}
