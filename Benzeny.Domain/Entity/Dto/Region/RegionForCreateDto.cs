
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Region
{
    public class RegionForCreateDto
    {
        public string Title { get; set; } = null!;
        [JsonIgnore] public string? CreatedBy { get; set; }
    }
}
