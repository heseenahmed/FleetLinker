using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Domain.Enums
{
    public enum Roles
    {
        [Display(Name = "Admin")]
        Admin = 1,
        [Display(Name = "Visitor")]
        Visitor,
        [Display(Name = "Equipment owner")]
        EquipmentOwner,
        [Display(Name = "Supplier")]
        Supplier,
        [Display(Name = "Maintenance workshop owner")]
        MaintenanceWorkshopOwner,
        [Display(Name = "driver")]
        Driver,
        [Display(Name = "mechanical")]
        Mechanical
    }
}
