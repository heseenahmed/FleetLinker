using System.ComponentModel.DataAnnotations;

namespace FleetLinker.Domain.Enums
{
    public enum PartType
    {
        [Display(Name = "Original")]
        Original = 1,
        [Display(Name = "Commercial")]
        Commercial = 2
    }
}
