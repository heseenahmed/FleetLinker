using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace FleetLinker.Domain.Entity.Dto
{
    public class ExcelDto
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
