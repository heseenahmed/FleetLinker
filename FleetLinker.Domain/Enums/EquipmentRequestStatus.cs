using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Domain.Enums
{
    public enum EquipmentRequestStatus
    {
        [Display(Name = "Pending")]
        Pending = 1,
        [Display(Name = "Responded")]
        Responded = 2,
        [Display(Name = "Accepted")]
        Accepted = 3,
        [Display(Name = "Rejected")]
        Rejected = 4
    }
}
