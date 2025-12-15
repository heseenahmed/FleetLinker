using System.ComponentModel.DataAnnotations;

namespace Benzeny.Domain.Entity.Dto.Identity
{
    public class RoleDto
    {

        [Required, StringLength(256)]
        [Display(Name = "Name")]
        public string Name { get; set; }
    }
}
