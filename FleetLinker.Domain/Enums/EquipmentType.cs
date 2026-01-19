using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Domain.Enums
{
    public enum EquipmentType
    {
        [Display(Name = "Truck")]
        Truck = 1,
        [Display(Name = "Equipment")]
        Equipment = 2
    }
}
