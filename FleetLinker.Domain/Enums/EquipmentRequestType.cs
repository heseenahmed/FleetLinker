using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Domain.Enums
{
    public enum EquipmentRequestType
    {
        [Display(Name = "Buy")]
        Buy = 1,
        [Display(Name = "Rent")]
        Rent = 2
    }
}
