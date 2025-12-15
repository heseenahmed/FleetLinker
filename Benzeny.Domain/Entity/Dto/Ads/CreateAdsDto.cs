
using BenzenyMain.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Serialization;

namespace BenzenyMain.Domain.Entity.Dto.Ads
{
    public class CreateAdsDto
    {
        public string Name { get; set; }
        public IFormFile Image { get; set; }
        public string? Url { get; set; }
        public string? Description { get; set; }
        public AdvertisementType Type { get; set; }
        public int DurationInMonths { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
