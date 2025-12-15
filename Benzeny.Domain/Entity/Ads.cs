
using Benzeny.Domain.Entity;
using BenzenyMain.Domain.Enum;

namespace BenzenyMain.Domain.Entity
{
    public class Ads:BaseEntity
    {
        public string Name { get; set; }

        public string Image { get; set; }

        public string? Url { get; set; }

        public string? Description { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public AdvertisementType Type { get; set; }

        public int DurationInMonths { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
