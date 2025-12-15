
using Microsoft.AspNetCore.Http;

namespace BenzenyMain.Domain.Entity.Dto.Car
{
    public class ImportCarExcelRequest
    {
        public IFormFile ExcelFile { get; set; } = null!;
        //public string UserId { get; set; } = null!;
        public Guid BranchId { get; set; }
    }
}
