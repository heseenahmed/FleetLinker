using System.ComponentModel.DataAnnotations;

namespace Benzeny.Domain.Entity.Dto.Identity
{
    public class RegisterRequest
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; } 
    }
    
}
