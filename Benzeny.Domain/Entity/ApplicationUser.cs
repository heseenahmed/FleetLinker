
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;
using BenzenyMain.Domain.Entity;

namespace Benzeny.Domain.Entity
{
    public enum DeviceType
    {
        [Display(Name = "Apple iOS")]
        IOS = 1,
        [Display(Name = "Android")]
        Android,
        [Display(Name = "WEB")]
        WEB,

    }
    public enum UserType
    {
        [Display(Name = "Administrator")]
        Administrator,
        [Display(Name = "Customer Support")]
        CustomerSupport,
        [Display(Name = "Marketer")]
        Marketer,
        [Display(Name = "Merchandiser")]
        Merchandiser,
        [Display(Name = "Online Store Editor")]
        OnlineStoreEditor
    }
    public enum Roles
    {
        [Display(Name = "Administrator")]
        Administrator,
        [Display(Name = "Customer Support")]
        CustomerSupport,
        [Display(Name = "Marketer")]
        Marketer,
        [Display(Name = "Merchandiser")]
        Merchandiser,
        [Display(Name = "Online Store Editor")]
        OnlineStoreEditor
    }
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "FullName")]
        public string FullName { get; set; } = null!;

        [Display(Name = "National ID")]
        public string? SSN { get; set; }

        public DeviceType DeviceType { get; set; }

        [Display(Name = "Refresh Token")]
        public string? RefreshToken { get; set; }

        [Display(Name = "Refresh Token Expiry UTC")]
        public DateTime? RefreshTokenExpiryUTC { get; set; }

        [Display(Name = "Deleted")]
        [DefaultValue(false)]
        public bool Deleted { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Birth Day")]
        public DateTime BirthDay { get; set; }
        [DefaultValue(true)]
        public bool FirstTimeLogin { get; set; }

        [DefaultValue(false)]
        public bool IsOTPEnabled { get; set; }
       
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
    }
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
     
    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() { }

        public ApplicationRole(string roleName)
            : base(roleName)
        {
        }
        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
    }
    public class ChangePasswordApi
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
    public class ForgetPasswordApi
    {

        [Required(ErrorMessage = "MobileNumberRequired")]
        [Display(Name = "Mobile")]
        [RegularExpression(@"^(\+\d{1,3}[- ]?)?\d{10}$", ErrorMessage = "MobileNumberRequired")]
        public string Mobile { get; set; }
        public string? Code { get; set; }
        public string? Password { get; set; }
    }
}
