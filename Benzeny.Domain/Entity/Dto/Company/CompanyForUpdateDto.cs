
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public class CompanyForUpdateDto
    {
        public Guid Id { get; set; }

        //company data
        public string? Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhone { get; set; }
        public List<IFormFile>? Files { get; set; }

        //user data
        public string Username { get; set; }
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string? SSN { get; set; }
        [Required]
        public string Password { get; set; }

        //[JsonIgnore]
        //public string? UpdatedBy { get; set; } = null!;
    }
}
