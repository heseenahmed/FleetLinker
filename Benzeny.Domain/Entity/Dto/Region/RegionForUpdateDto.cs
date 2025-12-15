
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Region
{
    public class RegionForUpdateDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        [JsonIgnore] public string? UpdatedBy { get; set; }
    }
}
