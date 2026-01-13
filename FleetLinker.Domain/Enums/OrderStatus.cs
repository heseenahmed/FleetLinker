using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Domain.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Pending Payment")]
        PendingPayment = 1,
        [Display(Name = "Paid")]
        Paid = 2,
        [Display(Name = "Shipped")]
        Shipped = 3,
        [Display(Name = "Completed")]
        Completed = 4,
        [Display(Name = "Cancelled")]
        Cancelled = 5
    }
}
