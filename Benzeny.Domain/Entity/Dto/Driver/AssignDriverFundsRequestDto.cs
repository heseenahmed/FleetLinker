
using BenzenyMain.Domain.Enum;

namespace BenzenyMain.Domain.Entity.Dto.Driver
{
    public class AssignDriverFundsRequestDto
    {
        public List<Guid> DriversIds { get; set; } = new();
        public decimal Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public LimitType? LimitType { get; set; }
        public List<Days>? Days { get; set; }
    }
}
