
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity
{
    public class CarDriver
    {
        public Guid CarId { get; set; }
        [JsonIgnore]
        public virtual Car Car { get; set; } = null!;

        public Guid DriverId { get; set; }
        [JsonIgnore]
        public virtual Driver Driver { get; set; } = null!;
    }
}
