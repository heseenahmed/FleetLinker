using System.ComponentModel.DataAnnotations;

namespace Benzeny.Domain.Entity.Dto.Identity
{
    public class RegisterRequest
    {
        //[Required]
        //public string Username { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Mobile { get; set; }
        //[Required]
        //public string Password { get; set; }
        //public bool IsOTPEnabled { get; set; } = false;
        //company related
        public Guid? CompanyId { get; set; } 
    }
    
}
