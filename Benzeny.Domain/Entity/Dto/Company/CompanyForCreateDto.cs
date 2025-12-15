
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Company
{
    public class CompanyForCreateDto
    {
        public string Name { get; set; }
        public IFormFile? CompanyPicture { get; set; }
        public string? Description { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyPhone { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
